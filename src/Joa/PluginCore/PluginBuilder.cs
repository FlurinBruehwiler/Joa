using Joa.Settings;
using Joa.Steps;
using JoaLauncher.Api;
using JoaLauncher.Api.Plugin;
using JoaLauncher.Api.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Joa.PluginCore;

public class PluginBuilder : IPluginBuilder
{
    private readonly PluginLoader _pluginLoader;
    private readonly PluginServiceProvider _pluginServiceProvider;

    public PluginBuilder(PluginLoader pluginLoader, PluginServiceProvider pluginServiceProvider)
    {
        _pluginLoader = pluginLoader;
        _pluginServiceProvider = pluginServiceProvider;
    }

    private readonly List<(Type, Func<string, bool>?)> _providers = new();
    private readonly List<SearchResult> _searchResults = new();
    private readonly List<SaveAction> _saveActions = new();

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

    public IPluginBuilder AddGlobalResult(SearchResult searchResult)
    {
        _searchResults.Add(searchResult);
        return this;
    }

    public IPluginBuilder AddSaveAction<T>(string nameOfProperty, Action<T> callback) where T : class
    {
        _saveActions.Add(new SaveAction(o => callback((T)o), null, typeof(T), nameOfProperty, false));
        return this;
    }

    public IPluginBuilder AddSaveAction<T>(string nameOfProperty, Func<T, Task> callback) where T : class
    {
        _saveActions.Add(new SaveAction(null, o => callback((T)o), typeof(T), nameOfProperty, true));
        return this;
    }

    public PluginDefinition BuildPluginDefinition(IPlugin plugin, ISetting setting, List<ICache> caches, List<IAsyncCache> asyncCaches, PluginManifest pluginManifest)
    {
        plugin.ConfigurePlugin(this);

        var globalProviders = InstantiateGlobalProviders().ToList();

        AddSearchResults(globalProviders);

        return new PluginDefinition(setting)
        {
            Plugin = plugin,
            Manifest = pluginManifest,
            GlobalProviders = globalProviders,
            Caches = caches,
            AsyncCaches = asyncCaches,
            SaveActions = _saveActions
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
}