using JoaCore.Settings;
using JoaCore.Step;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;

namespace JoaCore.PluginCore;

public class PluginManager
{
    public List<PluginDefinition> Plugins { get; set; } = new();
    public List<ProviderWrapper> GlobalProviders { get; set; } = new();

    private SettingsManager SettingsManager { get; set; }
    private readonly PluginLoader _pluginLoader;
    private readonly IJoaLogger _logger;

    public PluginManager(SettingsManager settingsManager, PluginLoader pluginLoader, IJoaLogger logger, PluginServiceProvider pluginServiceProvider)
    {
        SettingsManager = settingsManager;
        _pluginLoader = pluginLoader;
        _logger = logger;
    }

    public List<T> GetPluginsOfType<T>() where T : IPlugin
    {
        return GetPluginDefinitionsOfType<T>().Select(x => (T)x.Plugin).ToList();
    }
    
    private List<PluginDefinition> GetPluginDefinitionsOfType<T>() where T : IPlugin
    {
        return Plugins.Where(x => x.Plugin is T).ToList();
    }
    
    public void ReloadPlugins()
    {
        Plugins = _pluginLoader.ReloadPlugins();
        GlobalProviders = Plugins.SelectMany(x => x.GlobalProviders).ToList();
        UpdateIndexes();
    }
    
    private void UpdateIndexes()
    {
        foreach (var cache in Plugins.SelectMany(x => x.Caches))
        {
            try
            {
                cache.UpdateIndexes();
            }
            catch (Exception e)
            {
                _logger.LogException(e, $"Updating the index for cache {cache.GetType().Name} failed");
            }
        }
    }
}