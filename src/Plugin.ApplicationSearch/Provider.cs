using JoaPluginsPackage;
using JoaPluginsPackage.Providers;

namespace ApplicationSearch;

public class Provider : IProvider
{
    private readonly Cache _cache;

    public Provider(Cache cache)
    {
        _cache = cache;
    }

    public List<ISearchResult> GetSearchResults(string searchString)
    {
        return _cache.SearchResults;
    }
}