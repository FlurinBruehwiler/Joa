using JoaPluginsPackage;
using JoaPluginsPackage.Enums;

namespace JoaCore;

public class PluginGenericSearchResultProvider : ISearchResultProvider
{
    public List<ISearchResult> SearchResults { get; set; }
    public SearchResultLifetime SearchResultLifetime { get; set; }
    public void UpdateSearchResults(ISearchProviderContext searchProviderContext)
    {
        throw new NotImplementedException();
    }
}