using JoaCore.SearchEngine;
using JoaPluginsPackage;

namespace JoaCore;

public class Search
{
    private readonly PluginManager _pluginManager;
    private readonly PluginServiceProvider _pluginServiceProvider;
    private ProviderWrapper _currentProvider;

    public Search(PluginManager pluginManager, PluginServiceProvider pluginServiceProvider)
    { 
        _pluginManager = pluginManager;
        _pluginServiceProvider = pluginServiceProvider;
        _pluginManager.ReloadPlugins();
        StringMatcher.Instance = new StringMatcher();
    }

    public async Task ExecuteCommand(Guid commandId, string actionKey)
    {
        var pluginSearchResult = _currentProvider.LastSearchResults?
            .FirstOrDefault(x => x.CommandId == commandId);

        if (pluginSearchResult is null)
            return;

        var contextAction = GetContextAction(actionKey, pluginSearchResult);
        
        if (contextAction is null)
            return;

        var executionContext = new ExecutionContext
        {
            ContextAction = contextAction,
            ServiceProvider = _pluginServiceProvider.ServiceProvider
        };
        
        await Task.Run(() => pluginSearchResult.SearchResult.Execute(executionContext));
    }

    private ContextAction? GetContextAction(string actionKey, PluginSearchResult pluginSearchResult)
    {
        if (actionKey == "enter")
        {
            return new ContextAction
            {
                Key = "enter"
            };
        }

        return pluginSearchResult.SearchResult.Actions?.SingleOrDefault(x => x.Key == actionKey);
    }

    public async Task UpdateSearchResults(string searchString,
        Action<List<PluginSearchResult>> callback)
    {
        // if (_currentProvider.Provider.SearchResultLifetime == SearchResultLifetime.Key)
        // {
        //     _currentProvider.Provider.GetSearchResults(null);
        //     callback(_currentProvider.Provider.SearchResults.ToPluginSerachResults());
        // }
        // else
        // {
        //     callback(SortSearchResults(_currentProvider.Provider.SearchResults.ToPluginSerachResults(), searchString));
        // }
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