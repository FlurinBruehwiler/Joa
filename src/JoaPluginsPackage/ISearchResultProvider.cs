using JoaPluginsPackage.Enums;

namespace JoaPluginsPackage;

public interface ISearchResultProvider
{
    public List<ISearchResult> SearchResults { get; set; }
    public SearchResultLifetime SearchResultLifetime { get; set; }
    public void UpdateSearchResults(ISearchProviderContext searchProviderContext);
}