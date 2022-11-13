using JoaPluginsPackage;
using JoaPluginsPackage.Providers;

namespace JoaInterface.PluginCore;

public class PluginGenericProvider : IProvider
{
    public List<ISearchResult> SearchResults { get; set; }
    
    public List<ISearchResult> GetSearchResults(string searchString)
    {
        return SearchResults;
    }
}