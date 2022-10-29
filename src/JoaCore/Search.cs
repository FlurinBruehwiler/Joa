using JoaCore.SearchEngine;
using JoaPluginsPackage;
using JoaPluginsPackage.Enums;

namespace JoaCore;

public class Search
{
    private readonly PluginManager _pluginManager;
    private readonly ServiceProviderForPlugins _serviceProvider;
    private SearchResultProviderWrapper _currentProvider;

    public Search(PluginManager pluginManager, ServiceProviderForPlugins serviceProvider)
    { 
        _pluginManager = pluginManager;
        _serviceProvider = serviceProvider;

        _pluginManager.ReloadPlugins();
        
        
        StringMatcher.Instance = new StringMatcher();
    }

    public async Task ExecuteCommand(Guid commandId, string actionKey)
    {
        var pluginCommand = _currentProvider.LastSearchResults?
            .FirstOrDefault(x => x.CommandId == commandId);

        if (pluginCommand is null)
            return;

        ContextAction? contextAction;
        
        if (actionKey == "enter")
        {
            contextAction = new ContextAction
            {
                Key = "enter"
            };
        }
        else
        {
            contextAction = pluginCommand.SearchResult.Actions?.SingleOrDefault(x => x.Key == actionKey);
        }

        if (contextAction is null)
            return;

        var executionContext = new ExecutionContext
        {
            ContextAction = contextAction,
            ServiceProvider = _serviceProvider.ServiceProvider
        };
        
        await Task.Run(() => pluginCommand.SearchResult.Execute(executionContext));
    }

    public async Task UpdateSearchResults(string searchString,
        Action<List<PluginSearchResult>> callback)
    {
        if (_currentProvider.Provider.SearchResultLifetime == SearchResultLifetime.Search)
        {
            _currentProvider.Provider.UpdateSearchResults(null);
            callback(_currentProvider.Provider.SearchResults.ToPluginSerachResults());
        }
        else
        {
            callback(SortSearchResults(_currentProvider.Provider.SearchResults.ToPluginSerachResults(), searchString));
        }
    }

    private List<PluginSearchResult> SortSearchResults(List<PluginSearchResult> input, string searchString)
    {
        var sortValues = input.Select(x => (x, StringMatcher.FuzzySearch( searchString,x.SearchResult.Caption).Score)).ToList();
        
        sortValues.Sort((x, y) =>
        {
            if (x.Item2 > y.Item2)
                return -1;
            return x.Item2 < y.Item2 ? 1 : 0;
        });

        return sortValues.Select(x => x.x).ToList();
    }
}