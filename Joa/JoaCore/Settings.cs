using Interfaces;

namespace JoaCore;

public class Settings
{
    public CoreSettings CoreSettings { get; set; }
    public IEnumerable<PluginDefinition> PluginDefinitions { get; set; }
    
    public Settings(CoreSettings coreSettings)
    {
        CoreSettings = coreSettings;
        PluginDefinitions = new List<PluginDefinition>();
    }

    public void UpdatePluginDefinitions(IEnumerable<IPlugin> plugins)
    {
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