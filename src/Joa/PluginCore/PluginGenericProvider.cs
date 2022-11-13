using Joa.Api;
using Joa.Api.Providers;

namespace JoaInterface.PluginCore;

public class PluginGenericProvider : IProvider
{
    public required List<ISearchResult> SearchResults { get; set; }
    
    public List<ISearchResult> GetSearchResults(string searchString)
    {
        return SearchResults;
    }
}