using JoaPluginsPackage;
using JoaPluginsPackage.Enums;

namespace Github.Providers;

public class RepositoriesProvider : ISearchResultProvider
{
    public List<ISearchResult> SearchResults { get; set; }
    public SearchResultLifetime SearchResultLifetime { get; set; }
    public void UpdateSearchResults(string searchString)
    {
        throw new NotImplementedException();
    }
}