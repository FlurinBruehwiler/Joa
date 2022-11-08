using JoaPluginsPackage;
using JoaPluginsPackage.Enums;
using JoaPluginsPackage.Providers;

namespace JoaCore;

public class PluginGenericSearchResultProvider : ISearchResultProvider
{
    public List<ISearchResult> SearchResults { get; set; }
    public SearchResultLifetime SearchResultLifetime { get; set; }
    public void UpdateSearchResults(string searchString)
    {
        throw new NotImplementedException();
    }
}