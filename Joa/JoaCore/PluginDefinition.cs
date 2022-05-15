using System.Reflection;
using Interfaces;
using Interfaces.Settings;

namespace JoaCore;

public class PluginDefinition
{
    public SettingsCollection SettingsCollection { get; set; }
    public string Name { get; set; }
    
    public PluginDefinition(IPlugin plugin)
    {
        Name = plugin.GetType().Name;
        SettingsCollection = new SettingsCollection(plugin);
    }
}