using System.Diagnostics;
using System.Text.Json;
using JoaInterface.PluginCore;
using JoaPluginsPackage;
using JoaPluginsPackage.Injectables;
using Microsoft.Extensions.Configuration;

namespace JoaInterface.Settings;

public class SettingsManager
{
    private readonly PluginManager _pluginManager;
    private readonly IJoaLogger _logger;
    private readonly string _settingsLocation;
    private readonly JsonSerializerOptions _options;
    private Stopwatch _timeSinceLastChanged;
    private Stopwatch _timeSinceLastSinc;

    public SettingsManager(PluginManager pluginManager, IConfiguration configuration, IJoaLogger logger)
    {
        logger.Info(nameof(SettingsManager));
        _pluginManager = pluginManager;
        _logger = logger;
        _timeSinceLastChanged = Stopwatch.StartNew();
        _timeSinceLastSinc = Stopwatch.StartNew();
        _settingsLocation = Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location, configuration.GetValue<string>("SettingsLocation")));
        _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        ConfigureFileWatcher();
        Sync();
    }
    
    private void ConfigureFileWatcher()
    {
        _logger.Log($"Setting up file watcher for the file {_settingsLocation}", IJoaLogger.LogLevel.Info);
        var watcher = new FileSystemWatcher(Directory.GetParent(_settingsLocation)?.FullName ?? throw new Exception("Error while getting SettingsLocation"));
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Changed += OnChanged;
        watcher.Filter = Path.GetFileName(_settingsLocation);
        watcher.EnableRaisingEvents = true;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (_timeSinceLastChanged.ElapsedMilliseconds < 100)
            return;
        if (_timeSinceLastSinc.ElapsedMilliseconds < 1000)
            return;
        _timeSinceLastChanged.Restart();
        Sync();
    }
    
    public void Sync()
    {
        _timeSinceLastSinc.Restart();
        _logger.Log("Synchronizing the settings.", IJoaLogger.LogLevel.Info);
        Thread.Sleep(10);
        UpdateSettingsFromJson();
        SaveSettingsToJson();
    }
    
    public void SaveSettingsToJson()
    {
        using var _ = _logger.TimedOperation(nameof(SaveSettingsToJson));
        
        try
        {
            var dtoSetting = new DtoSettings(_pluginManager.Plugins);
            var jsonString = JsonSerializer.Serialize(dtoSetting, _options);
            File.WriteAllText(_settingsLocation, jsonString);
        }
        catch (Exception e)
        {
            _logger.Log($"There was an exception thrown while Saving the Settings with the following exception{Environment.NewLine}{e}", IJoaLogger.LogLevel.Error);
        }
    }

    public void UpdateSettingsFromJson()
    {
        using var _ = _logger.TimedOperation(nameof(UpdateSettingsFromJson));
        
        try
        {
            if (!File.Exists(_settingsLocation))
            {
                File.Create(_settingsLocation).Dispose();
            }
            var jsonString = File.ReadAllText(_settingsLocation);
            if (string.IsNullOrEmpty(jsonString))
                return;
            var result = JsonSerializer.Deserialize<DtoSettings>(jsonString);
            if (result is null)
                throw new JsonException();
        
            foreach (var pluginDefinition in _pluginManager.Plugins)
            {
                UpdatePluginDefinition(pluginDefinition, result);
            }
        }
        catch (Exception e)
        {
            _logger.Log($"There was an exception thrown while Updating the Settings from the settings.json with the following exception{Environment.NewLine}{e}", IJoaLogger.LogLevel.Error);
        }
    }

    private void UpdatePluginDefinition(PluginDefinition pluginDefinition, DtoSettings newDtoSettings)
    {
        if (!newDtoSettings.Plugins.TryGetValue(pluginDefinition.PluginInfo.Name, out var newPlugin))
            return;

        var x = newPlugin.Setting.Deserialize(pluginDefinition.Setting.GetType());

        if (x is null)
        {
            _logger.Error("Something went wrong while reading the settings");
            return;
        }

        pluginDefinition.Setting = (ISetting)x;
    }
}