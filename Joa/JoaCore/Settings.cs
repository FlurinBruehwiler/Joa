using System.Text.Json;
using Interfaces;

namespace JoaCore;

public class Settings
{
    public CoreSettings CoreSettings { get; set; }
    public List<PluginDefinition> PluginDefinitions { get; set; }
    
    
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;

    
    public Settings(CoreSettings coreSettings, IEnumerable<IPlugin> plugins)
    {
        CoreSettings = coreSettings;
        PluginDefinitions = CreatePluginDefinitions(plugins).ToList();
        _filePath = Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location,
            @"..\..\..\..\..\settings.json"));
        _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }
    private IEnumerable<PluginDefinition> CreatePluginDefinitions(IEnumerable<IPlugin> plugins)
    {
        foreach (var plugin in plugins)
        {
            yield return new PluginDefinition(plugin);
        }
    }
    
    public void SaveSettingsToJson()
    {
        //var dtoSetting = new DtoSettings(this);
        //var jsonString = JsonSerializer.Serialize(dtoSetting, _options);
        //File.WriteAllText(_filePath, jsonString);
    }

    public void UpdateSettingsFromJson()
    {
        var jsonString = File.ReadAllText(_filePath);
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
        // var newPlugin = dtoSettings.PluginSettings[pluginDefinition.Name];
        //
        // foreach (var pluginSetting in pluginDefinition.PluginSettings)
        // {
        //     pluginSetting.Value = newPlugin[pluginSetting.Name];
        // }
    }
}