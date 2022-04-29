using PluginBase;

namespace AppWithPlugin;

public class Search
{
    public List<ISearchResult> SearchResults { get; set; }

    private readonly PluginLoader _pluginLoader;

    public Search()
    {
        SearchResults = new List<ISearchResult>();
        _pluginLoader = new PluginLoader();
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
            pluginsTasks.Remove(pluginTask);
        }
    }
}