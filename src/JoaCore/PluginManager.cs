using JoaCore.PluginCore;
using JoaCore.Settings;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;

namespace JoaCore;

public class PluginManager
{
    public List<PluginDefinition> Plugins { get; set; } = new();
    public List<ProviderWrapper> Providers { get; set; } = new();

    private SettingsManager SettingsManager { get; set; }
    private readonly PluginLoader _pluginLoader;
    private readonly IJoaLogger _logger;
    private readonly ServiceProviderForPlugins _serviceProvider;

    public PluginManager(SettingsManager settingsManager, PluginLoader pluginLoader, IJoaLogger logger, ServiceProviderForPlugins serviceProvider)
    {
        SettingsManager = settingsManager;
        _pluginLoader = pluginLoader;
        _logger = logger;
        _serviceProvider = serviceProvider;
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
        using var _ = _logger.TimedOperation(nameof(ReloadPlugins));
    
        Plugins = new List<PluginDefinition>();
        
        foreach (var plugin in _pluginLoader.InstantiatePlugins().ToList())
        {
            var pluginBuilder = new PluginBuilder(_logger, _serviceProvider);
            var pluginDefinition = pluginBuilder.BuildPluginDefinition(plugin);
            Plugins.Add(pluginDefinition);
        }

        Providers = Plugins.SelectMany(x => x.GlobalProviders).ToList();
        
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