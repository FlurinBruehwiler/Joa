using JoaCore.PluginCore;
using JoaCore.Settings;
using JoaPluginsPackage.Enums;
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
    
    public PluginManager(SettingsManager settingsManager, PluginLoader pluginLoader, IJoaLogger logger)
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
        using var _ = _logger.TimedOperation(nameof(ReloadPlugins));
    
        Plugins = new List<PluginDefinition>();
        
        foreach (var plugin in _pluginLoader.InstantiatePlugins().ToList())
        {
            var pluginBuilder = new PluginBuilder(_logger);
            var pluginDefinition = pluginBuilder.BuildPluginDefinition(plugin);
            Plugins.Add(pluginDefinition);
        }

        Providers = Plugins.SelectMany(x => x.GlobalProviders).ToList();
        
        UpdateIndexes();
    }
    
    private void UpdateIndexes()
    {
        // foreach (var provider in GetProvidersWithLifeTime(SearchResultLifetime.Interval))
        // {
        //     try
        //     {
        //         provider.Provider.GetSearchResults(string.Empty);
        //     }
        //     catch (Exception e)
        //     {
        //         _logger.LogException(e, $"Updating the index for provider {provider.Provider.GetType().Name} failed");
        //     }
        // }
    }
}