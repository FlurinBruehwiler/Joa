using System.Reflection;
using JoaCore.PluginCore;
using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore;

public class PluginBuilder : IPluginBuilder
{
    private readonly IJoaLogger _joaLogger;
    private readonly ServiceProviderForPlugins _serviceProvider;

    public PluginBuilder(IJoaLogger joaLogger, ServiceProviderForPlugins serviceProvider)
    {
        _joaLogger = joaLogger;
        _serviceProvider = serviceProvider;
    }
    
    private readonly List<(Type, Delegate?)> _providers = new();
    private readonly List<Type> _settings = new();
    private readonly List<ISearchResult> _searchResults = new();
    private readonly List<Type> _caches = new();

    public IPluginBuilder AddGlobalProvider<T>() where T : IProvider
    {
        _providers.Add((typeof(T), null));
        return this;
    }

    public IPluginBuilder AddGlobalProvider<T>(Delegate condition) where T : IProvider
    {
        _providers.Add((typeof(T), condition));
        return this;
    }

    public IPluginBuilder AddGlobalResult(ISearchResult searchResult)
    {
        _searchResults.Add(searchResult);
        return this;
    }

    public IPluginBuilder AddSetting<T>() where T : ISetting
    {
        _settings.Add(typeof(T));
        return this;
    }

    public IPluginBuilder AddCache<T>() where T : ICache
    {
        _caches.Add(typeof(T));
        return this;
    }

    public PluginDefinition BuildPluginDefinition(IPlugin plugin)
    {
        var pluginInfos = GetPluginInfos(plugin.GetType());
        
        if (pluginInfos is null)
            throw new Exception("Error while Building Plugin Definition");
        
        plugin.ConfigurePlugin(this);

        var settings = InstantiateSettings().ToList();

        var caches = InstantiateCaches().ToList();
        
        var globalProviders = InstantiateGlobalProviders().ToList();

        AddSearchResults(globalProviders);

        return new PluginDefinition
        {
            Plugin = plugin,
            PluginInfo = pluginInfos,
            GlobalProviders = globalProviders,
            Settings = settings,
            Caches = caches
        };
    }

    private IEnumerable<ISetting> InstantiateSettings()
    {
        foreach (var settingType in _settings)
        {
            if (ActivatorUtilities.CreateInstance(_serviceProvider.ServiceProvider, settingType) is not ISetting setting)
                continue;

            _serviceProvider.ServiceCollection.AddSingleton(setting);
            
            yield return setting;
        }
        
        _serviceProvider.BuildServiceProvider();
    }
    
    private IEnumerable<ICache> InstantiateCaches()
    {
        foreach (var cacheType in _caches)
        {
            if(ActivatorUtilities.CreateInstance(_serviceProvider.ServiceProvider, cacheType) is not ICache cache)
                continue;

            _serviceProvider.ServiceCollection.AddSingleton(cache);

            yield return cache;
        }
        
        _serviceProvider.BuildServiceProvider();
    }
    
    private IEnumerable<ProviderWrapper> InstantiateGlobalProviders()
    {
        foreach (var (type, condition) in _providers)
        {
            if (ActivatorUtilities.CreateInstance(_serviceProvider.ServiceProvider, type) is not IProvider searchResultProvider)
                continue;

            yield return new ProviderWrapper
            {
                Condition = condition,
                Provider = searchResultProvider
            };
        }
    }

    private void AddSearchResults(List<ProviderWrapper> providers)
    {
        var genericSearchResultProvider = new PluginGenericProvider
        {
            SearchResults = _searchResults
        };
        
        providers.Add(new ProviderWrapper
        {
            Provider = genericSearchResultProvider
        });
    }
    

    //ToDo should throw Exception
    private PluginAttribute? GetPluginInfos(MemberInfo pluginType)
    {
        if (Attribute.GetCustomAttributes(pluginType).FirstOrDefault(x => x is PluginAttribute) is PluginAttribute pluginAttribute)
            return pluginAttribute;
        
        _joaLogger.Log($"The plugin {pluginType.Name} does not have the PluginAttribute", IJoaLogger.LogLevel.Error);
        return null;
    }
}