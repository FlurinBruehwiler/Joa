using Interfaces;

namespace AppWithPlugin;

public class Search
{
    private List<ISearchResult> SearchResults { get; }

    private readonly PluginLoader _pluginLoader;
    
    public delegate void ResultsUpdatedDelegate(List<ISearchResult> results);
    public event ResultsUpdatedDelegate ResultsUpdated;
    public Settings Settings { get; set; }

    public Search()
    {
        Settings = new Settings();
        SearchResults = new List<ISearchResult>();
        _pluginLoader = new PluginLoader(Settings);
        Settings.UpdateSettings(_pluginLoader);
    }
    
    public async Task UpdateSearchResults(string searchString)
    {
        SearchResults.Clear();
        var pluginsTasks = new List<Task<IEnumerable<ISearchResult>>>();
        
        foreach (var plugin in _pluginLoader.GetPlugins())
        {
            var pluginTask = Task.Run(() => plugin.GetResults(searchString));
            pluginsTasks.Add(pluginTask);
        }
        
        while (pluginsTasks.Count > 0)
        {
            var pluginTask = await Task.WhenAny(pluginsTasks);
            var pluginResult = await pluginTask;
            SearchResults.AddRange(pluginResult);
            ResultsUpdated?.Invoke(SearchResults);
            pluginsTasks.Remove(pluginTask);
        }
    }
}