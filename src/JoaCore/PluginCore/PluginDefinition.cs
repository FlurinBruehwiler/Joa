using System.Text.Json.Serialization;
using JoaCore.Settings;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Plugin;

namespace JoaCore.PluginCore;

public class PluginDefinition
{
    public Guid Id { get; }


    [JsonIgnore]
    public IPlugin Plugin { get; set; }

    public SettingsCollection SettingsCollection { get; set; }
    public PluginAttribute PluginInfo { get; set; }
    public PluginDefinition(IPlugin plugin, PluginAttribute pluginInfo)
    {
        PluginInfo = pluginInfo;
        Plugin = plugin;
        Id = Guid.NewGuid();
        SettingsCollection = new SettingsCollection(plugin);
    }
}