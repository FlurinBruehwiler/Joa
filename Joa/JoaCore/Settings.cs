using Interfaces;

namespace JoaCore;

public class Settings
{
    public CoreSettings CoreSettings { get; set; }
    public IEnumerable<PluginDefinition> PluginDefinitions { get; set; }
    
    public Settings(CoreSettings coreSettings, IEnumerable<IPlugin> plugins)
    {
        CoreSettings = coreSettings;
        PluginDefinitions = CreatePluginDefinitions(plugins);
    }
    private IEnumerable<PluginDefinition> CreatePluginDefinitions(IEnumerable<IPlugin> plugins)
    {
        foreach (var plugin in plugins)
        {
            yield return new PluginDefinition(plugin);
        }
    }
}