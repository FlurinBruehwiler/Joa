using System.Reflection;
using JoaCore.PluginCore;
using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Enums;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore;

public class PluginBuilder : IPluginBuilder
{
    private readonly IJoaLogger _joaLogger;

    public PluginBuilder(IJoaLogger joaLogger)
    {
        _joaLogger = joaLogger;
    }
    
    private readonly List<(Type, bool isGlobal, Delegate?)> _providers = new();
    private readonly List<Type> _settings = new();
    private readonly List<Type> _searchResults = new();
    
    public IPluginBuilder AddGlobalProvider<T>() where T : ISearchResultProvider
    {
        _providers.Add((typeof(T), true, null));
        return this;
    }

    public IPluginBuilder AddGlobalProvider<T>(Delegate condition) where T : ISearchResultProvider
    {
        _providers.Add((typeof(T), true, condition));
        return this;
    }

    public IPluginBuilder AddGlobalResult(ISearchResult searchResult)
    {
        throw new NotImplementedException();
    }

    public IPluginBuilder AddProvider<T>() where T : ISearchResultProvider
    {
        _providers.Add((typeof(T), false, null));
        return this;
    }
    
    public IPluginBuilder AddSetting<T>() where T : ISetting
    {
        _settings.Add(typeof(T));
        return this;
    }

    public PluginDefinition BuildPluginDefinition(IPlugin plugin)
    {
        var pluginInfos = GetPluginInfos(plugin.GetType());
        
        if (pluginInfos is null)
            throw new Exception("Error while Building Plugin Definition");
        
        plugin.ConfigurePlugin(this);
        
        var searchResultProviders = InstantiateProviders();

        AddSearchResults(searchResultProviders);

        return new PluginDefinition
        {
            Plugin = plugin,
            PluginInfo = pluginInfos,
            SearchResultProviders = searchResultProviders
        };
    }

    private void AddSearchResults(List<SearchResultProviderWrapper> searchResultProviders)
    {
        var searchResults = new List<ISearchResult>();

        foreach (var searchResultType in _searchResults)
        {
            if (Activator.CreateInstance(searchResultType) is not ISearchResult searchResult)
                continue;

            searchResults.Add(searchResult);
        }

        var genericSearchResultProvider = new PluginGenericSearchResultProvider
        {
            SearchResults = searchResults,
            SearchResultLifetime = SearchResultLifetime.Session
        };

        searchResultProviders.Add(new SearchResultProviderWrapper
        {
            Provider = genericSearchResultProvider,
            IsGlobal = true
        });
    }

    private ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

        foreach (var settingType in _settings)
        {
            if (Activator.CreateInstance(settingType) is not ISetting setting)
                continue;
            services.AddSingleton(setting);
        }

        services.AddSingleton(_joaLogger);
        services.AddSingleton<IBrowserHelper, BrowserHelper>();
        services.AddSingleton<IIconHelper, IconHelper>();
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider;
    }

    private List<SearchResultProviderWrapper> InstantiateProviders()
    {
        var searchResultProviders = new List<SearchResultProviderWrapper>();
        
        var serviceProvider = CreateServiceProvider();
        
        foreach (var (type, isGlobal, condition) in _providers)
        {
            if (ActivatorUtilities.CreateInstance(serviceProvider, type) is not ISearchResultProvider searchResultProvider)
                continue;

            searchResultProviders.Add(new SearchResultProviderWrapper
            {
                Condition = condition,
                Provider = searchResultProvider,
                IsGlobal = isGlobal
            });
        }

        return searchResultProviders;
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