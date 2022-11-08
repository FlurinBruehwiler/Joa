using JoaPluginsPackage;
using JoaPluginsPackage.Providers;

namespace JoaCore;

public class PluginGenericProvider : IProvider
{
    public List<ISearchResult> SearchResults { get; set; }
    
    public List<ISearchResult> GetSearchResults(string searchString)
    {
        return SearchResults;
    }
}