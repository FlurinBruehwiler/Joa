using JoaPluginsPackage.Providers;

namespace JoaCore;

public class ProviderWrapper
{
    public ISearchResultProvider SearchResultProvider { get; set; } = null!;
    public Delegate? Condition { get; set; }
    public List<PluginSearchResult>? LastSearchResults { get; set; }
}