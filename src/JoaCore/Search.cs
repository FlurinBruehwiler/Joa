using JoaCore.Settings;
using JoaPluginsPackage.Plugin;
using Microsoft.Extensions.Configuration;

namespace JoaCore;

public class Search
{
    private SettingsManager SettingsManager { get; set; }
    private PluginManager PluginManager { get; set; }
    private List<PluginCommand>? _lastSearchResults;

    public Search(IConfiguration configuration)
    {
        SettingsManager = new SettingsManager(new CoreSettings(), configuration);
        PluginManager = new PluginManager(SettingsManager, configuration);
        PluginManager.ReloadPlugins();
    }

    public async Task ExecuteCommand(Guid commandId)
    {
        var pluginCommand = _lastSearchResults?
            .FirstOrDefault(x => x.CommandId == commandId);

        if (pluginCommand is null)
            return;
        
        var pluginDef = PluginManager.Plugins?
            .FirstOrDefault(p => p.Id == pluginCommand.PluginId);
        
        if (pluginDef is null)
            return;
        
        await Task.Run(() => pluginDef.Plugin.Execute(pluginCommand.Command));
    }

    public async Task<List<PluginCommand>> GetSearchResults(string searchString)
    {
        var timer = JoaLogger.GetInstance().StartMeasure();

        if (PluginManager.Plugins == null)
            return new List<PluginCommand>();

        _lastSearchResults = new List<PluginCommand>();

        (IStrictPlugin strictPlugin, Guid id)? matchingPlugin = GetMatchingPlugin(searchString);

        if (matchingPlugin.HasValue)
        {
            var strictPluginResult = await Task.Run(() => matchingPlugin.Value.strictPlugin.GetResults(searchString));
            _lastSearchResults.AddRange(strictPluginResult.Select(x => new PluginCommand(x, matchingPlugin.Value.id)));
            return _lastSearchResults;
        }

        foreach (var pluginDefinition in PluginManager.Plugins)
        {
            if(pluginDefinition.Plugin is not IIndexablePlugin indexablePlugin)
                continue;

            foreach (var searchResult in indexablePlugin.SearchResults)
            {
                _lastSearchResults.Add(new PluginCommand(searchResult, pluginDefinition.Id));
            }
        }
        
        SortSearchResults(_lastSearchResults);
        
        JoaLogger.GetInstance().LogMeasureResult(timer,$"{nameof(GetSearchResults)}:{searchString}");

        return _lastSearchResults;
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