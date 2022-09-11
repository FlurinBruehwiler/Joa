using JoaCore.PluginCore;
using JoaCore.SearchEngine;
using JoaCore.Settings;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;
using Microsoft.Extensions.Configuration;

namespace JoaCore;

public class Search
{
    private readonly JoaLogger _logger;
    private readonly PluginManager _pluginManager;
    private List<PluginSearchResult>? _lastSearchResults;

    public Search(JoaLogger logger, PluginManager pluginManager)
    { 
        _logger = logger;
        _pluginManager = pluginManager;
        
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
        var pluginDef = _pluginManager.GetPluginDefinitionsOfType<ISearchPlugin>() 
            .FirstOrDefault(plugin => plugin.Id == pluginCommand.PluginId);
        
        if (pluginDef is null)
            return;

        var action = pluginCommand.SearchResult.Actions?.SingleOrDefault(x => x.Key == actionKey);

        if (action is null)
            return;
        
        await Task.Run(() => pluginDef.Plugin.GetTypedPlugin<ISearchPlugin>().Execute(pluginCommand.SearchResult, action));
    }

    public async Task UpdateSearchResults(string searchString,
        Action<List<PluginSearchResult>> callback)
    {
        var stopwatch = _logger.StartMeasure();

        if (string.IsNullOrWhiteSpace(searchString))
        {
            callback(new List<PluginSearchResult>());
            return;
        }

        if (_pluginManager.Plugins == null)
        {
            callback(new List<PluginSearchResult>());
            return;
        }

        _lastSearchResults = new List<PluginSearchResult>();

        var matchingPluginDefinition = GetMatchingPlugin(searchString);

        if (matchingPluginDefinition is not null)
        {
            var strictPluginResult = await Task.Run(() => matchingPluginDefinition.Plugin.GetTypedPlugin<IStrictSearchPlugin>().GetStrictSearchResults(searchString));
            _lastSearchResults.AddRange(strictPluginResult.Select(x => new PluginSearchResult
            {
                SearchResult = x,
                PluginId = matchingPluginDefinition.Id
            }));
            _logger.LogMeasureResult(stopwatch, "Search");
            callback(_lastSearchResults);
            return;
        }

        foreach (var plugin in _pluginManager.GetPluginDefinitionsOfType<IGlobalSearchPlugin>())
        {
            foreach (var searchResult in plugin.Plugin.GetTypedPlugin<IGlobalSearchPlugin>().GlobalSearchResults)
            {
                _lastSearchResults.Add(new PluginSearchResult
                {
                    SearchResult = searchResult,
                    PluginId = plugin.Id
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

    private PluginDefinition? GetMatchingPlugin(string searchString)
    {
        if (_pluginManager.Plugins == null)
            return null;

        foreach (var pluginDefinition in _pluginManager.GetPluginDefinitionsOfType<IStrictSearchPlugin>())
        {
            if (pluginDefinition.Plugin.GetTypedPlugin<IStrictSearchPlugin>().Validator(searchString))
                return pluginDefinition;
        }

        return null;
    }
}