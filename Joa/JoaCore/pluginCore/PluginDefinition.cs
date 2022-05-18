using Interfaces;

namespace JoaCore.pluginCore;

public class PluginDefinition
{
    public Guid ID { get; }
    public IPlugin Plugin { get; set; }
    public SettingsCollection SettingsCollection { get; set; }
    public string Name { get; set; }
    
    public PluginDefinition(IPlugin plugin)
    {
        ID = new Guid();
        Plugin = plugin;
        Name = plugin.GetType().Name;
        SettingsCollection = new SettingsCollection(plugin);
    }
}