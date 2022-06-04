using JoaCore.Settings;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Settings.Attributes;

namespace JoaCore.PluginCore;

public class PluginDefinition
{
    public Guid Id { get; }
    public IPlugin Plugin { get; set; }
    public SettingsCollection SettingsCollection { get; set; }
    public PluginAttribute PluginInfo { get; set; }
    public PluginDefinition(IPlugin plugin, PluginAttribute pluginInfo)
    {
        PluginInfo = pluginInfo;
        Id = new Guid();
        Plugin = plugin;
        if (string.IsNullOrEmpty(PluginInfo.Name))
        {
            PluginInfo.Name = plugin.GetType().Name;
        }
        SettingsCollection = new SettingsCollection(plugin);
    }
}