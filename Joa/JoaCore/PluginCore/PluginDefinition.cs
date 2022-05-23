using Interfaces.Plugin;
using Interfaces.Settings.Attributes;
using JoaCore.Settings;

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