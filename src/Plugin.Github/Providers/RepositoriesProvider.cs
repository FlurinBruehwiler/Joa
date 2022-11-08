using JoaPluginsPackage;
using JoaPluginsPackage.Enums;
using JoaPluginsPackage.Providers;

namespace Github.Providers;

public class RepositoriesProvider : IProvider
{
    public List<ISearchResult> SearchResults { get; set; }
    public SearchResultLifetime SearchResultLifetime { get; set; }
    public List<ISearchResult> GetSearchResults(string searchString)
    {
        throw new NotImplementedException();
    }
}