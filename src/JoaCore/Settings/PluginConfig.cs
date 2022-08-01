using JoaPluginsPackage.Attributes;

namespace JoaCore.Settings;

public class PluginConfig
{
    public PluginConfig(PluginAttribute pluginAttribute, IEnumerable<PluginSetting> pluginSettings)
    {
        PluginAttribute = pluginAttribute;
        PluginSettings = pluginSettings;
    }

    public PluginAttribute PluginAttribute { get; set; }
    public IEnumerable<PluginSetting> PluginSettings { get; set; }
}