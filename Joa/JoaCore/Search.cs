using Interfaces;
using Interfaces.Logger;
using JoaCore.pluginCore;
using JoaCore.settings;
using Microsoft.Extensions.Logging;

namespace JoaCore;

public class Search
{ 
    public delegate void ResultsUpdatedDelegate(List<(ISearchResult, Guid)> results);
    public event ResultsUpdatedDelegate? ResultsUpdated;

    public Settings Settings { get; set; }
    private List<PluginDefinition> Plugins { get; set; }
    
    private readonly PluginLoader _pluginLoader;
    private readonly CoreSettings _coreSettings;
    private readonly ILogger<IJoaLogger> _logger;

    private List<(ISearchResult, Guid)> SearchResults { get; }

    public Search()
    {
        SearchResults = new List<(ISearchResult, Guid)>();
        _pluginLoader = new PluginLoader();
        _coreSettings = new CoreSettings();
        _logger = new Logger<IJoaLogger>(new LoggerFactory());
        Load();
    }

    public void Load()
    {
        Plugins = new();
        foreach (var plugin in _pluginLoader.InstantiatePlugins(_coreSettings).ToList())
        {
            Plugins.Add(new PluginDefinition(plugin));
        }
        Settings = new Settings(_coreSettings, Plugins);
    }
    
    public async Task ExecuteSearchResult(Guid pluginId, ISearchResult searchResult)
    {
        foreach (var pluginDef in Plugins.Where(p => p.ID == pluginId))
        {
            pluginDef.Plugin.Execute(searchResult);
        }
    }

    public async Task UpdateSearchResults(string searchString)
    {
        SearchResults.Clear();
        var pluginsTasks = new Dictionary<Task<List<ISearchResult>>, Guid>();

        foreach (var pluginDef in Plugins)
        {
            var plugin = pluginDef.Plugin;
            var pluginTask = Task.Run(() => plugin.GetResults(searchString));
            pluginsTasks.Add(pluginTask, pluginDef.ID);
        }

        while (pluginsTasks.Count > 0)
        {
            var pluginTask = await Task.WhenAny(pluginsTasks.Keys);
            var pluginResult = await pluginTask;
            foreach (var searchResult in pluginResult)
            {
                SearchResults.Add((searchResult, pluginsTasks[pluginTask]));
            }
            ResultsUpdated?.Invoke(SearchResults);
            pluginsTasks.Remove(pluginTask);
        }
    }
}