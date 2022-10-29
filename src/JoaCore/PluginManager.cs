using JoaCore.PluginCore;
using JoaCore.Settings;
using JoaPluginsPackage.Enums;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;

namespace JoaCore;

public class PluginManager
{
    public List<PluginDefinition>? Plugins { get; set; }
    private SettingsManager SettingsManager { get; set; }
    private readonly PluginLoader _pluginLoader;
    private readonly IJoaLogger _logger;
    public List<SearchResultProviderWrapper>? Providers { get; set; }


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

    public List<PluginDefinition> GetPluginDefinitionsOfType<T>() where T : IPlugin
    {
        if (Plugins is null)
            return new List<PluginDefinition>();

        return Plugins.Where(x => x.Plugin is T).ToList();
    }
    
    public void ReloadPlugins()
    {
        var timer = _logger.StartMeasure();
    
        Plugins = new List<PluginDefinition>();
        
        foreach (var plugin in _pluginLoader.InstantiatePlugins().ToList())
        {
            var pluginBuilder = new PluginBuilder(_logger);
            var pluginDefinition = pluginBuilder.BuildPluginDefinition(plugin);
            Plugins.Add(pluginDefinition);
        }

        Providers = Plugins.SelectMany(x => x.SearchResultProviders).ToList();
        
        // IntervalProviders = Providers
        //     .Where(x => x.IsGlobal && x.Provider.SearchResultLifetime == SearchResultLifetime.Interval)
        //     .ToList();
        //
        // StrictProviders = Providers.Where(x => x.IsGlobal && x.Condition is not null).ToList();
        
        UpdateIndexes();
        
        _logger.LogMeasureResult(timer, nameof(ReloadPlugins));
    }
    
    private void UpdateIndexes()
    {
        var context = new GlobalSearchProviderContext();
        
        // foreach (var provider in IntervalProviders)
        // {
        //     try
        //     {
        //         provider.Provider.UpdateSearchResults(context);
        //     }
        //     catch (Exception e)
        //     {
        //         _logger.LogException(e, $"Updating the index for provider {provider.GetType().Name} failed");
        //     }
        // }
    }
}