using System.Text.Json;
using Interfaces.Logger;
using JoaCore.PluginCore;

namespace JoaCore.Settings;

public class SettingsManager
{
    public List<PluginDefinition> PluginDefinitions { get; set; }
    private CoreSettings CoreSettings { get; set; }

    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;

    public SettingsManager(CoreSettings coreSettings, List<PluginDefinition> pluginDefs)
    {
        _filePath = Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location,
            @"..\..\..\..\..\settings.json"));
        _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        Load(coreSettings, pluginDefs);
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
            File.WriteAllText(_filePath, jsonString);
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
            var jsonString = File.ReadAllText(_filePath);
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

    private void UpdatePluginDefinition(PluginDefinition pluginDefinition, DtoSettings dtoSettings)
    {
        if (!dtoSettings.PluginSettings.TryGetValue(pluginDefinition.Name, out var newPlugin))
            return;
        
        foreach (var pluginSetting in pluginDefinition.SettingsCollection.PluginSettings)
        {
            pluginSetting.Value = newPlugin[pluginSetting.Name];
        }
    }
}