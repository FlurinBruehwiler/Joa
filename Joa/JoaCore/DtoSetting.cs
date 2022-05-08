namespace JoaCore;

public class DtoSetting
{
    public Dictionary<string, Dictionary<string, object>> PluginSettings { get; set; }
    
    public DtoSetting(Settings setting)
    {
        PluginSettings = setting.PluginDefinitions.ToDictionary(pluginDefinition => pluginDefinition.Name,
            pluginDefinition => pluginDefinition.PluginSettings.ToDictionary(pluginSetting => pluginSetting.Name,
                pluginSetting => pluginSetting.Value));
    }
}