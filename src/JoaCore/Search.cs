using FuzzySharp;
using JoaCore.Settings;
using JoaPluginsPackage.Logger;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Plugin.Search;
using Microsoft.Extensions.Configuration;

namespace JoaCore;

public class Search
{
    public SettingsManager SettingsManager { get; set; }
    private PluginManager PluginManager { get; set; }
    private List<PluginCommand>? _lastSearchResults;

    public Search(IConfiguration configuration)
    {
        SettingsManager = new SettingsManager(new CoreSettings(), configuration);
        PluginManager = new PluginManager(SettingsManager, configuration);
        PluginManager.ReloadPlugins();
    }

    public async Task ExecuteCommand(Guid commandId, string actionKey)
    {
        var pluginCommand = _lastSearchResults?
            .FirstOrDefault(x => x.CommandId == commandId);

        if (pluginCommand is null)
            return;
        
        JoaLogger.GetInstance().Log(pluginCommand.PluginId.ToString(), IJoaLogger.LogLevel.Info);
        var pluginDef = PluginManager.GetPluginsOfType<ISearchPlugin>() 
            .FirstOrDefault(plugin => plugin.Id == pluginCommand.PluginId);
        
        if (pluginDef is null)
            return;

        var action = pluginCommand.SearchResult.Actions.SingleOrDefault(x => x.Key == actionKey);

        if (action is null)
            return;
        
        await Task.Run(() => pluginDef.Execute(pluginCommand.SearchResult, action));
    }

    public async Task<List<PluginCommand>> GetSearchResults(string searchString)
    {
        var timer = JoaLogger.GetInstance().StartMeasure();

        if (PluginManager.Plugins == null)
            return new List<PluginCommand>();

        _lastSearchResults = new List<PluginCommand>();

        (IStrictSearchPlugin strictPlugin, Guid id)? matchingPlugin = GetMatchingPlugin(searchString);

        if (matchingPlugin.HasValue)
        {
            var strictPluginResult = await Task.Run(() => matchingPlugin.Value.strictPlugin.GetStrictSearchResults(searchString));
            _lastSearchResults.AddRange(strictPluginResult.Select(x => new PluginCommand(x, matchingPlugin.Value.id)));
            return _lastSearchResults;
        }

        foreach (var plugin in PluginManager.GetPluginsOfType<IGlobalSearchPlugin>())
        {
            foreach (var searchResult in plugin.GlobalSearchResults)
            {
                _lastSearchResults.Add(new PluginCommand(searchResult, plugin.Id));
                JoaLogger.GetInstance().Log(plugin.Id.ToString(), IJoaLogger.LogLevel.Info);
            }
        }
        
        return SortSearchResults(_lastSearchResults, searchString);
    }

    private List<PluginCommand> SortSearchResults(List<PluginCommand> input, string searchString)
    {
        var sortValues = input.Select(x => (x, Fuzz.Ratio(x.SearchResult.Caption, searchString))).ToList();
        
        sortValues.Sort((x, y) =>
        {
            if (x.Item2 > y.Item2)
                return -1;
            return x.Item2 < y.Item2 ? 1 : 0;
        });

        return sortValues.Select(x => x.x).ToList();
    }

    private (IStrictSearchPlugin, Guid)? GetMatchingPlugin(string searchString)
    {
        if (PluginManager.Plugins == null)
            return null;

        foreach (var plugin in PluginManager.GetPluginsOfType<IStrictSearchPlugin>())
        {
            if (plugin.Validator(searchString))
                return (plugin, plugin.Id);
        }

        return null;
    }
}