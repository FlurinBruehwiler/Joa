using System.Reflection;
using JoaCore.PluginCore;
using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Enums;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore;

public class PluginBuilder : IPluginBuilder
{
    private readonly IJoaLogger _joaLogger;

    public PluginBuilder(IJoaLogger joaLogger)
    {
        _joaLogger = joaLogger;
    }
    
    private readonly List<(Type, Delegate?)> _providers = new();
    private readonly List<Type> _settings = new();
    private readonly List<ISearchResult> _searchResults = new();
    
    public IPluginBuilder AddGlobalProvider<T>() where T : IResultProvider
    {
        _providers.Add((typeof(T), null));
        return this;
    }

    public IPluginBuilder AddGlobalProvider<T>(Delegate condition) where T : IResultProvider
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

    public PluginDefinition BuildPluginDefinition(IPlugin plugin)
    {
        var pluginInfos = GetPluginInfos(plugin.GetType());
        
        if (pluginInfos is null)
            throw new Exception("Error while Building Plugin Definition");
        
        plugin.ConfigurePlugin(this);
        
        var globalProviders = InstantiateGlobalProviders();

        AddSearchResults(globalProviders);

        return new PluginDefinition
        {
            Plugin = plugin,
            PluginInfo = pluginInfos,
            GlobalProviders = globalProviders
        };
    }

    private void AddSearchResults(List<SearchResultProviderWrapper> searchResultProviders)
    {
        var genericSearchResultProvider = new PluginGenericResultProvider
        {
            SearchResults = _searchResults,
            SearchResultLifetime = SearchResultLifetime.Session
        };

        searchResultProviders.Add(new SearchResultProviderWrapper
        {
            Provider = genericSearchResultProvider
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

    private List<SearchResultProviderWrapper> InstantiateGlobalProviders()
    {
        var providers = new List<SearchResultProviderWrapper>();
        
        var serviceProvider = CreateServiceProvider();
        
        foreach (var (type, condition) in _providers)
        {
            if (ActivatorUtilities.CreateInstance(serviceProvider, type) is not IResultProvider searchResultProvider)
                continue;

            providers.Add(new SearchResultProviderWrapper
            {
                Condition = condition,
                Provider = searchResultProvider
            });
        }

        return providers;
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