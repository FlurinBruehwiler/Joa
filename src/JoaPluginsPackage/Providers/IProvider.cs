using JoaPluginsPackage.Enums;

namespace JoaPluginsPackage.Providers;

public interface IProvider
{
    public List<ISearchResult> SearchResults { get; set; }
    public SearchResultLifetime SearchResultLifetime { get; set; }
    public void UpdateSearchResults(string searchString);
}