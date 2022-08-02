using JoaCore.PluginCore;
using JoaCore.SearchEngine;
using JoaCore.Settings;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;
using Microsoft.Extensions.Configuration;

namespace JoaCore;

public class Search
{
    public SettingsManager SettingsManager { get; set; }
    private PluginManager PluginManager { get; set; }
    private List<PluginSearchResult>? _lastSearchResults;

    private JoaLogger _logger = JoaLogger.GetInstance();

    public Search(IConfiguration configuration)
    {
        SettingsManager = new SettingsManager(new CoreSettings(), configuration);
        PluginManager = new PluginManager(SettingsManager, configuration);
        PluginManager.ReloadPlugins();
        StringMatcher.Instance = new StringMatcher();
    }

    public async Task ExecuteCommand(Guid commandId, string actionKey)
    {
        var pluginCommand = _lastSearchResults?
            .FirstOrDefault(x => x.CommandId == commandId);

        if (pluginCommand is null)
            return;
        
        _logger.Log(pluginCommand.PluginId.ToString(), IJoaLogger.LogLevel.Info);
        var pluginDef = PluginManager.GetPluginDefinitionsOfType<ISearchPlugin>() 
            .FirstOrDefault(plugin => plugin.Id == pluginCommand.PluginId);
        
        if (pluginDef is null)
            return;

        var action = pluginCommand.SearchResult.Actions.SingleOrDefault(x => x.Key == actionKey);

        if (action is null)
            return;
        
        await Task.Run(() => pluginDef.Plugin.GetTypedPlugin<ISearchPlugin>().Execute(pluginCommand.SearchResult, action));
    }

    public async Task<List<PluginSearchResult>> GetSearchResults(string searchString)
    {
        var stopwatch = _logger.StartMeasure();
        
        if (string.IsNullOrWhiteSpace(searchString))
            return new List<PluginSearchResult>();

        if (PluginManager.Plugins == null)
            return new List<PluginSearchResult>();

        _lastSearchResults = new List<PluginSearchResult>();

        var matchingPluginDefinition = GetMatchingPlugin(searchString);

        if (matchingPluginDefinition is not null)
        {
            var strictPluginResult = await Task.Run(() => matchingPluginDefinition.Plugin.GetTypedPlugin<IStrictSearchPlugin>().GetStrictSearchResults(searchString));
            _lastSearchResults.AddRange(strictPluginResult.Select(x => new PluginSearchResult(x, matchingPluginDefinition.Id)));
            _logger.LogMeasureResult(stopwatch, "Search");
            return _lastSearchResults;
        }

        foreach (var plugin in PluginManager.GetPluginDefinitionsOfType<IGlobalSearchPlugin>())
        {
            foreach (var searchResult in plugin.Plugin.GetTypedPlugin<IGlobalSearchPlugin>().GlobalSearchResults)
            {
                _lastSearchResults.Add(new PluginSearchResult(searchResult, plugin.Id));
            }
        }
        
        var res = SortSearchResults(_lastSearchResults, searchString);

        _logger.LogMeasureResult(stopwatch, "Search");
        
        return res;
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
        if (PluginManager.Plugins == null)
            return null;

        foreach (var pluginDefinition in PluginManager.GetPluginDefinitionsOfType<IStrictSearchPlugin>())
        {
            if (pluginDefinition.Plugin.GetTypedPlugin<IStrictSearchPlugin>().Validator(searchString))
                return pluginDefinition;
        }

        return null;
    }
}