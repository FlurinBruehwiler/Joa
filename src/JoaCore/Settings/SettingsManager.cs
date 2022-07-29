using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using JoaCore.PluginCore;
using JoaPluginsPackage.Logger;
using Microsoft.Extensions.Configuration;

namespace JoaCore.Settings;

public class SettingsManager
{
    private readonly IConfiguration _configuration;
    public List<PluginDefinition> PluginDefinitions { get; set; } = null!;
    
    [JsonIgnore]
    public CoreSettings CoreSettings { get; set; }

    private readonly string _settingsLocation;
    private readonly JsonSerializerOptions _options;
    private Stopwatch _timeSinceLastChanged;
    private Stopwatch _timeSinceLastSinc;

    public SettingsManager(CoreSettings coreSettings, IConfiguration configuration)
    {
        _configuration = configuration;
        _timeSinceLastChanged = Stopwatch.StartNew();
        _timeSinceLastSinc = Stopwatch.StartNew();
        _settingsLocation = Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location, configuration.GetValue<string>("SettingsLocation")));
        _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        CoreSettings = coreSettings;
        ConfigureFileWatcher();
    }

    public void LoadPluginSettings(List<PluginDefinition> pluginDefs)
    {
        PluginDefinitions = pluginDefs;
        Sync();
    }

    private void ConfigureFileWatcher()
    {
        JoaLogger.GetInstance().Log($"Setting up file watcher for the file {_settingsLocation}", IJoaLogger.LogLevel.Info);
        var watcher = new FileSystemWatcher(Directory.GetParent(_settingsLocation)?.FullName ?? throw new Exception("Error while getting SettingsLocation"));
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Changed += OnChanged;
        watcher.Filter = Path.GetFileName(_settingsLocation);
        watcher.EnableRaisingEvents = true;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        JoaLogger.GetInstance().Log("The settings File has been changed.", IJoaLogger.LogLevel.Info);
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
        JoaLogger.GetInstance().Log("Synchronizing the settings.", IJoaLogger.LogLevel.Info);
        Thread.Sleep(10);
        UpdateSettingsFromJson();
        SaveSettingsToJson();
    }
    
    public void SaveSettingsToJson()
    {
        var timer = JoaLogger.GetInstance().StartMeasure();
        
        try
        {
            var dtoSetting = new DtoSettings(this);
            var jsonString = JsonSerializer.Serialize(dtoSetting, _options);
            File.WriteAllText(_settingsLocation, jsonString);
        }
        catch (Exception e)
        {
            JoaLogger.GetInstance().Log($"There was an exception thrown while Saving the Settings with the following exception{Environment.NewLine}{e}", IJoaLogger.LogLevel.Error);
        }
        
        JoaLogger.GetInstance().LogMeasureResult(timer ,nameof(SaveSettingsToJson));
    }

    public void UpdateSettingsFromJson()
    {
        var timer = JoaLogger.GetInstance().StartMeasure();
        
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
        
            foreach (var pluginDefinition in PluginDefinitions)
            {
                UpdatePluginDefinition(pluginDefinition, result);
            }
        }
        catch (Exception e)
        {
            JoaLogger.GetInstance().Log($"There was an exception thrown while Updating the Settings from the settings.json with the following exception{Environment.NewLine}{e}", IJoaLogger.LogLevel.Error);
        }
        
        JoaLogger.GetInstance().LogMeasureResult(timer, nameof(UpdateSettingsFromJson));
    }

    private void UpdatePluginDefinition(PluginDefinition oldPluginDefinition, DtoSettings newDtoSettings)
    {
        if (!newDtoSettings.PluginSettings.TryGetValue(oldPluginDefinition.PluginInfo.Name, out var newPlugin))
            return;
        
        foreach (var pluginSetting in oldPluginDefinition.SettingsCollection.PluginSettings)
        {
            if(!newPlugin.TryGetValue(pluginSetting.Name, out var newValue))
                continue;

            pluginSetting.Value = newValue;
        }
    }
}