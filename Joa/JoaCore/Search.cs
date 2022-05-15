using Interfaces;

namespace JoaCore;

public class Search
{ 
    public delegate void ResultsUpdatedDelegate(List<(ISearchResult, Guid)> results);
    public event ResultsUpdatedDelegate? ResultsUpdated;

    public Settings Settings { get; set; }
    private List<IPlugin> Plugins { get; set; }
    
    private readonly PluginLoader _pluginLoader;
    private readonly CoreSettings _coreSettings;
    
    private List<(ISearchResult, Guid)> SearchResults { get; }

    public Search()
    {
        SearchResults = new List<(ISearchResult, Guid)>();
        _pluginLoader = new PluginLoader();
        _coreSettings = new CoreSettings();
        Load();
    }

    public void Load()
    {
        Plugins = _pluginLoader.InstantiatePlugins(_coreSettings).ToList();
        Settings = new Settings(_coreSettings, Plugins);
    }
    
    public async Task ExecuteSearchResult(Guid pluginId, ISearchResult searchResult)
    {
        foreach (var plugin in Plugins.Where(p => p.ID == pluginId))
        {
            plugin.Execute(searchResult);
        }
    }

    public async Task UpdateSearchResults(string searchString)
    {
        SearchResults.Clear();
        var pluginsTasks = new Dictionary<Task<List<ISearchResult>>, Guid>();

        foreach (var plugin in Plugins)
        {
            var pluginTask = Task.Run(() => plugin.GetResults(searchString));
            pluginsTasks.Add(pluginTask, plugin.ID);
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