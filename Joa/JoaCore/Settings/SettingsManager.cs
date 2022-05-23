using System.Text.Json;
using Interfaces.Logger;
using JoaCore.PluginCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

namespace JoaCore.Settings;

public class SettingsManager
{
    private readonly IConfiguration _configuration;
    public List<PluginDefinition> PluginDefinitions { get; set; } = null!;
    private CoreSettings CoreSettings { get; set; } = null!;

    private readonly string _settingsLocation;
    private readonly JsonSerializerOptions _options;

    public SettingsManager(CoreSettings coreSettings, List<PluginDefinition> pluginDefs, IConfiguration configuration)
    {
        _configuration = configuration;
        _settingsLocation = Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location, configuration.GetValue<string>("SettingsLocation")));
        _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        Load(coreSettings, pluginDefs);
        ConfigureFileWatcher();
    }

    private void ConfigureFileWatcher()
    {
        LoggingManager.JoaLogger.Log($"Setting up file watcher for the file {_settingsLocation}", IJoaLogger.LogLevel.Info);
        var watcher = new FileSystemWatcher(Directory.GetParent(_settingsLocation)?.FullName ?? throw new Exception("Error while getting SettingsLocation"));
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Changed += OnChanged;
        watcher.Filter = Path.GetFileName(_settingsLocation);
        watcher.EnableRaisingEvents = true;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        LoggingManager.JoaLogger.Log("The settings File has been changed", IJoaLogger.LogLevel.Info);
        Sync();
    }

    public void Load(CoreSettings coreSettings, List<PluginDefinition> pluginDefs)
    {
        CoreSettings = coreSettings;
        PluginDefinitions = pluginDefs;
        Sync();
    }

    public void Sync()
    {
        UpdateSettingsFromJson();
        SaveSettingsToJson();
    }
    
    public void SaveSettingsToJson()
    {
        try
        {
            var dtoSetting = new DtoSettings(this);
            var jsonString = JsonSerializer.Serialize(dtoSetting, _options);
            File.WriteAllText(_settingsLocation, jsonString);
        }
        catch (Exception e)
        {
            LoggingManager.JoaLogger.Log($"There was an exception thrown while Saving the Settings with the following exception{Environment.NewLine}{e}", IJoaLogger.LogLevel.Error);
        }
    }

    public void UpdateSettingsFromJson()
    {
        try
        {
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
            LoggingManager.JoaLogger.Log($"There was an exception thrown while Updating the Settings from the settings.json with the following exception{Environment.NewLine}{e}", IJoaLogger.LogLevel.Error);
        }
    }

    private void UpdatePluginDefinition(PluginDefinition oldPluginDefinition, DtoSettings newDtoSettings)
    {
        if (!newDtoSettings.PluginSettings.TryGetValue(oldPluginDefinition.Name, out var newPlugin))
            return;
        
        foreach (var pluginSetting in oldPluginDefinition.SettingsCollection.PluginSettings)
        {
            if(!newPlugin.TryGetValue(pluginSetting.Name, out var newValue))
                continue;

            pluginSetting.Value = newValue;
        }
    }
}