using JoaPluginsPackage.Enums;

namespace JoaPluginsPackage.Providers;

public interface ISearchResultProvider
{
    public SearchResultLifetime SearchResultLifetime { get; set; }
    public IEnumerable<ISearchResult> GetSearchResults(string searchString);
}