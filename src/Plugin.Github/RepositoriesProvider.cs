using JoaPluginsPackage;
using JoaPluginsPackage.Enums;

namespace Github;

public class RepositoriesProvider : ISearchResultProvider
{
    public List<ISearchResult> SearchResults { get; set; }
    public SearchResultLifetime SearchResultLifetime { get; set; }
    public ISearchProviderContext Context { get; set; }
    public void UpdateSearchResults(ISearchProviderContext context)
    {
        throw new NotImplementedException();
    }
}