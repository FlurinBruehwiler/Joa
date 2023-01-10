using JoaLauncher.Api;
using JoaLauncher.Api.Providers;

namespace Joa.PluginCore;

public class PluginGenericProvider : IProvider
{
    public required List<SearchResult> SearchResults { get; set; }

    public List<SearchResult> GetSearchResults(string searchString)
    {
        return SearchResults;
    }
}