using Interfaces.Plugin;
using JoaCore.Settings;

namespace JoaCore.PluginCore;

public class PluginDefinition
{
    public Guid Id { get; }
    public IPlugin Plugin { get; set; }
    public SettingsCollection SettingsCollection { get; set; }
    public string Name { get; set; }
    
    public PluginDefinition(IPlugin plugin)
    {
        Id = new Guid();
        Plugin = plugin;
        Name = plugin.GetType().Name;
        SettingsCollection = new SettingsCollection(plugin);
    }
}