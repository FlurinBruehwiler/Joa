using System.Reflection;
using JoaInterface.Step;
using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace JoaInterface.PluginCore;

public class PluginBuilder : IPluginBuilder
{
    private readonly PluginLoader _pluginLoader;
    private readonly IJoaLogger _joaLogger;
    private readonly PluginServiceProvider _pluginServiceProvider;

    public PluginBuilder(PluginLoader pluginLoader, IJoaLogger joaLogger, PluginServiceProvider pluginServiceProvider)
    {
        _pluginLoader = pluginLoader;
        _joaLogger = joaLogger;
        _pluginServiceProvider = pluginServiceProvider;
    }
    
    private readonly List<(Type, Func<string, bool>?)> _providers = new();
    private readonly List<ISearchResult> _searchResults = new();

    public IPluginBuilder AddGlobalProvider<T>() where T : IProvider
    {
        _providers.Add((typeof(T), null));
        return this;
    }

    public IPluginBuilder AddGlobalProvider<T>(Func<string, bool> condition) where T : IProvider
    {
        _providers.Add((typeof(T), condition));
        return this;
    }

    public IPluginBuilder AddGlobalResult(ISearchResult searchResult)
    {
        _searchResults.Add(searchResult);
        return this;
    }

    public PluginDefinition BuildPluginDefinition(IPlugin plugin, ISetting setting, List<ICache> caches)
    {
        var pluginInfos = GetPluginInfos(plugin.GetType());
        
        if (pluginInfos is null)
            throw new Exception("Error while Building Plugin Definition");
        
        plugin.ConfigurePlugin(this);

        var globalProviders = InstantiateGlobalProviders().ToList();

        AddSearchResults(globalProviders);

        return new PluginDefinition
        {
            Plugin = plugin,
            PluginInfo = pluginInfos,
            GlobalProviders = globalProviders,
            Setting = setting,
            Caches = caches
        };
    }

    
    private IEnumerable<ProviderWrapper> InstantiateGlobalProviders()
    {
        foreach (var (providerType, condition) in _providers)
        {
            if (ActivatorUtilities.CreateInstance(_pluginServiceProvider.ServiceProvider, providerType) is not IProvider searchResultProvider)
                continue;

            if (_pluginLoader.TryGetExistingObject<IProvider>(providerType, out var provider))
            {
                searchResultProvider = provider ?? throw new Exception();
            }
            
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