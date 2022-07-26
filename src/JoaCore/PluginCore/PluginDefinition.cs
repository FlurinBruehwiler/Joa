using System.Text.Json.Serialization;
using JoaCore.Settings;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Settings.Attributes;

namespace JoaCore.PluginCore;

public class PluginDefinition<T> where T : IPlugin
{
    public Guid Id { get; }

    [JsonIgnore]
    public T Plugin { get; set; }

    public SettingsCollection SettingsCollection { get; set; }
    public PluginAttribute PluginInfo { get; set; }
    public PluginDefinition(T plugin, PluginAttribute pluginInfo)
    {
        PluginInfo = pluginInfo;
        Id = Guid.NewGuid();
        Plugin = plugin;
        if (string.IsNullOrEmpty(PluginInfo.Name))
        {
            PluginInfo.Name = plugin.GetType().Name;
        }
        SettingsCollection = new SettingsCollection(plugin);
    }
}