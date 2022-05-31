using Interfaces;
using Interfaces.Plugin;
using JoaCore.Settings;
using Microsoft.Extensions.Configuration;

namespace JoaCore;

public class Search
{
    public delegate void ResultsUpdatedDelegate(List<(ISearchResult, Guid)> results);
    public event ResultsUpdatedDelegate ResultsUpdated;
    private SettingsManager SettingsManager { get; set; }
    private List<(ISearchResult, Guid)> SearchResults { get; }
    private PluginManager PluginManager { get; set; }

    public Search(IConfiguration configuration)
    {
        SearchResults = new List<(ISearchResult, Guid)>();
        SettingsManager = new SettingsManager(new CoreSettings(), configuration);
        PluginManager = new PluginManager(SettingsManager, configuration);
        PluginManager.ReloadPlugins();
    }

    public async Task ExecuteSearchResult(Guid pluginId, ISearchResult searchResult)
    {
        var pluginDef = PluginManager.Plugins?.First(p => p.Id == pluginId);
        if (pluginDef is null)
            return;
        await Task.Run(() => pluginDef.Plugin.Execute(searchResult)); //ToDo Check if it is really async
    }

    public async Task UpdateSearchResults(string searchString)
    {
        var timer = JoaLogger.GetInstance().StartMeasure();

        if (PluginManager.Plugins == null)
            return;

        SearchResults.Clear();

        (IStrictPlugin strictPlugin, Guid id)? matchingPlugin = GetMatchingPlugin(searchString);

        if (matchingPlugin.HasValue)
        {
            var strictPluginResult = await Task.Run(() => matchingPlugin.Value.strictPlugin.GetResults(searchString));
            SearchResults.AddRange(strictPluginResult.Select(x => (x, matchingPlugin.Value.id)));
            return;
        }

        foreach (var pluginDefinition in PluginManager.Plugins)
        {
            if(pluginDefinition.Plugin is not IIndexablePlugin indexablePlugin)
                continue;

            foreach (var searchResult in indexablePlugin.SearchResults)
            {
                SearchResults.Add((searchResult, pluginDefinition.Id));
            }
        }

        ResultsUpdated.Invoke(SearchResults);

        JoaLogger.GetInstance().LogMeasureResult(timer,$"{nameof(UpdateSearchResults)}:{searchString}");
    }

    private (IStrictPlugin, Guid)? GetMatchingPlugin(string searchString)
    {
        if (PluginManager.Plugins == null)
            return null;

        foreach (var pluginDefinition in PluginManager.Plugins)
        {
            if(pluginDefinition.Plugin is not IStrictPlugin strictPlugin)
                continue;

            if (strictPlugin.Validator(searchString))
                return (strictPlugin, pluginDefinition.Id);
        }

        return null;
    }
}