using System.Text.Json;
using JoaCore.pluginCore;

namespace JoaCore.settings;

public class Settings
{
    public List<PluginDefinition> PluginDefinitions { get; set; }
    private CoreSettings CoreSettings { get; set; }

    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;

    public Settings(CoreSettings coreSettings, List<PluginDefinition> pluginDefs)
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
        var dtoSetting = new DtoSettings(this);
        var jsonString = JsonSerializer.Serialize(dtoSetting, _options);
        File.WriteAllText(_filePath, jsonString);
    }

    public void UpdateSettingsFromJson()
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

    private void UpdatePluginDefinition(PluginDefinition pluginDefinition, DtoSettings dtoSettings)
    {
        var newPlugin = dtoSettings.PluginSettings[pluginDefinition.Name];
        
        foreach (var pluginSetting in pluginDefinition.SettingsCollection.PluginSettings)
        {
            pluginSetting.Value = newPlugin[pluginSetting.Name];
        }
    }
}