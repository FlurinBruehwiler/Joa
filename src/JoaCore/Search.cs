using JoaCore.Settings;
using JoaPluginsPackage.Plugin;
using Microsoft.Extensions.Configuration;

namespace JoaCore;

public class Search
{
    private SettingsManager SettingsManager { get; set; }
    
    private PluginManager PluginManager { get; set; }

    public Search(IConfiguration configuration)
    {
        SettingsManager = new SettingsManager(new CoreSettings(), configuration);
        PluginManager = new PluginManager(SettingsManager, configuration);
        PluginManager.ReloadPlugins();
    }

    public async Task ExecuteCommand(Guid pluginId, ICommand command)
    {
        var pluginDef = PluginManager.Plugins?.First(p => p.Id == pluginId);
        if (pluginDef is null)
            return;
        await Task.Run(() => pluginDef.Plugin.Execute(command)); //ToDo Check if it is really async
    }

    public async Task<List<PluginCommand>> GetSearchResults(string searchString)
    {
        var timer = JoaLogger.GetInstance().StartMeasure();

        if (PluginManager.Plugins == null)
            return new List<PluginCommand>();

        List<PluginCommand> searchResults = new();

        (IStrictPlugin strictPlugin, Guid id)? matchingPlugin = GetMatchingPlugin(searchString);

        if (matchingPlugin.HasValue)
        {
            var strictPluginResult = await Task.Run(() => matchingPlugin.Value.strictPlugin.GetResults(searchString));
            searchResults.AddRange(strictPluginResult.Select(x => new PluginCommand(x, matchingPlugin.Value.id)));
            return searchResults;
        }

        foreach (var pluginDefinition in PluginManager.Plugins)
        {
            if(pluginDefinition.Plugin is not IIndexablePlugin indexablePlugin)
                continue;

            foreach (var searchResult in indexablePlugin.SearchResults)
            {
                searchResults.Add(new PluginCommand(searchResult, pluginDefinition.Id));
            }
        }
        
        SortSearchResults(searchResults);
        
        JoaLogger.GetInstance().LogMeasureResult(timer,$"{nameof(GetSearchResults)}:{searchString}");

        return searchResults;
    }

    private void SortSearchResults(List<PluginCommand> input)
    {
        //ToDo SortResults
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