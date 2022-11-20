using JoaLauncher.Api;
using JoaLauncher.Api.Providers;

namespace Joa.PluginCore;

public class PluginGenericProvider : IProvider
{
    public required List<ISearchResult> SearchResults { get; set; }
    
    public List<ISearchResult> GetSearchResults(string searchString)
    {
        return SearchResults;
    }
}