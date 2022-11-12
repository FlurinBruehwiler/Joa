using System.Reflection;
using JoaCore.Step;
using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore.PluginCore;

public class PluginBuilder : IPluginBuilder
{
    private readonly IJoaLogger _joaLogger;
    private readonly PluginServiceProvider _pluginServiceProvider;

    public PluginBuilder(IJoaLogger joaLogger, PluginServiceProvider pluginServiceProvider)
    {
        _joaLogger = joaLogger;
        _pluginServiceProvider = pluginServiceProvider;
    }
    
    private readonly List<(Type, Func<string, bool>?)> _providers = new();
    private readonly List<Type> _settings = new();
    private readonly List<ISearchResult> _searchResults = new();
    private readonly List<Type> _caches = new();

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

    public PluginDefinition BuildPluginDefinition(IPlugin plugin, List<ISetting> settings, List<ICache> caches)
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
            Settings = settings,
            Caches = caches
        };
    }

    
    private IEnumerable<ProviderWrapper> InstantiateGlobalProviders()
    {
        foreach (var (type, condition) in _providers)
        {
            if (ActivatorUtilities.CreateInstance(_pluginServiceProvider.ServiceProvider, type) is not IProvider searchResultProvider)
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