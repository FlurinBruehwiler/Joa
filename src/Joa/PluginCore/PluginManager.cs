using Joa.Step;
using JoaLauncher.Api.Injectables;
using JoaLauncher.Api.Plugin;

namespace Joa.PluginCore;

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
        ReloadPlugins().GetAwaiter().GetResult();
    }

    public List<T> GetPluginsOfType<T>() where T : IPlugin
    {
        return GetPluginDefinitionsOfType<T>().Select(x => (T)x.Plugin).ToList();
    }
    
    private List<PluginDefinition> GetPluginDefinitionsOfType<T>() where T : IPlugin
    {
        return Plugins.Where(x => x.Plugin is T).ToList();
    }
    
    public async Task ReloadPlugins()
    {
        Plugins = _pluginLoader.ReloadPlugins();
        GlobalProviders = Plugins.SelectMany(x => x.GlobalProviders).ToList();
        await UpdateIndexesAsync();
    }

    private async Task UpdateIndexesAsync()
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
        
        foreach (var asyncCache in Plugins.SelectMany(x => x.AsyncCaches))
        {
            try
            {
                await asyncCache.UpdateIndexesAsync();
            }
            catch (Exception e)
            {
                _logger.LogException(e, $"Updating the index for async cache {asyncCache.GetType().Name} failed");
            }
        }
    }
}