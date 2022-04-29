using PluginBase;

namespace AppWithPlugin;

public class Search
{
    public List<ISearchResult> SearchResults { get; set; }

    public Search()
    {
        SearchResults = new();
    }
    
    public async Task UpdateSearchResults(List<IPlugin> plugins, string searchString)
    {
        SearchResults.Clear();
        var pluginsTasks = new List<Task<List<ISearchResult>>>();
        
        foreach (var plugin in plugins)
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