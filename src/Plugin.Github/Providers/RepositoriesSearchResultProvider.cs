using JoaPluginsPackage;
using JoaPluginsPackage.Enums;
using JoaPluginsPackage.Providers;

namespace Github.Providers;

public class RepositoriesSearchResultProvider : ISearchResultProvider
{
    public List<ISearchResult> SearchResults { get; set; }
    public SearchResultLifetime SearchResultLifetime { get; set; }
    public IEnumerable<ISearchResult> GetSearchResults(string searchString)
    {
        throw new NotImplementedException();
    }
}