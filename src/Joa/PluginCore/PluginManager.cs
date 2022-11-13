using JoaInterface.Step;
using Joa.Api.Injectables;
using Joa.Api.Plugin;

namespace JoaInterface.PluginCore;

public class PluginManager
{
    public List<PluginDefinition> Plugins { get; set; } = new();
    public List<ProviderWrapper> GlobalProviders { get; set; } = new();

    private readonly PluginLoader _pluginLoader;
    private readonly IJoaLogger _logger;

    public PluginManager(PluginLoader pluginLoader, IJoaLogger logger)
    {
        logger.Info(nameof(PluginManager));
        _pluginLoader = pluginLoader;
        _logger = logger;
        ReloadPlugins();
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