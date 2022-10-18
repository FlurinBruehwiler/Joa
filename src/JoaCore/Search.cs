using JoaCore.PluginCore;
using JoaCore.SearchEngine;
using JoaPluginsPackage;
using JoaPluginsPackage.Injectables;

namespace JoaCore;

public class Search
{
    private readonly IJoaLogger _logger;
    private readonly PluginManager _pluginManager;
    private readonly ServiceProviderForPlugins _serviceProvider;
    private List<PluginSearchResult>? _lastSearchResults;
    private List<PluginSearchResult>? _currentContextResults;

    public Search(IJoaLogger logger, PluginManager pluginManager, ServiceProviderForPlugins serviceProvider)
    { 
        _logger = logger;
        _pluginManager = pluginManager;
        _serviceProvider = serviceProvider;

        _pluginManager.ReloadPlugins();
        
        StringMatcher.Instance = new StringMatcher();
    }

    public async Task ExecuteCommand(Guid commandId, string actionKey)
    {
        var pluginCommand = _lastSearchResults?
            .FirstOrDefault(x => x.CommandId == commandId);

        if (pluginCommand is null)
            return;
        
        _logger.Log(pluginCommand.PluginId.ToString(), IJoaLogger.LogLevel.Info);
        var pluginDef = _pluginManager.Providers
            .FirstOrDefault(provider => provider.Id == pluginCommand.PluginId);
        
        if (pluginDef is null)
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
        var stopwatch = _logger.StartMeasure();

        _lastSearchResults = new List<PluginSearchResult>();
        
        if (string.IsNullOrWhiteSpace(searchString) || _pluginManager.Plugins is null)
        {
            callback(_lastSearchResults);
            return;
        }

        if (_currentContextResults is not null)
        {
            var contextRes = SortSearchResults(_currentContextResults, searchString);
            callback(contextRes);
            return;
        }
        
        var matchingProvider = GetMatchingProvider(searchString);

        if (matchingProvider is not null)
        {
            await Task.Run(() => matchingProvider.Provider.UpdateSearchResults(null));

            var strictPluginResult = matchingProvider.Provider.SearchResults;
            
            _lastSearchResults.AddRange(strictPluginResult.Select(x => new PluginSearchResult
            {
                SearchResult = x,
                PluginId = matchingProvider.Id
            }));
            _logger.LogMeasureResult(stopwatch, "Search");
            callback(_lastSearchResults);
            return;
        }

        foreach (var provider in _pluginManager.NonStrictProviders)
        {
            foreach (var searchResult in provider.Provider.SearchResults)
            {
                _lastSearchResults.Add(new PluginSearchResult
                {
                    SearchResult = searchResult,
                    PluginId = provider.Id
                });
            }
        }
        
        var res = SortSearchResults(_lastSearchResults, searchString);

        _logger.LogMeasureResult(stopwatch, "Search");
        
        callback(res);
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

    private SearchResultProviderWrapper? GetMatchingProvider(string searchString)
    {
        if (_pluginManager.Plugins == null)
            return null;

        foreach (var provider in _pluginManager.StrictProviders)
        {
            // if (provider.Condition(searchString))
            //     return pluginDefinition; 
            //ToDo
        }

        return null;
    }
}