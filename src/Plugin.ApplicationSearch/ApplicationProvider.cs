using JoaPluginsPackage;
using JoaPluginsPackage.Providers;

namespace ApplicationSearch;

public class ApplicationProvider : IProvider
{
    private readonly ApplicationSearch _applicationSearch;

    public ApplicationProvider(ApplicationSearch applicationSearch)
    {
        _applicationSearch = applicationSearch;
    }

    public List<ISearchResult> GetSearchResults(string searchString)
    {
        return _applicationSearch.CachedResults;
    }
}