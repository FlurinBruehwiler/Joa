using JoaPluginsPackage;
using JoaPluginsPackage.Providers;

namespace JoaCore;

public class SearchResultProviderWrapper
{
    public IResultProvider Provider { get; set; } = null!;
    public Delegate? Condition { get; set; }
    public List<PluginSearchResult>? LastSearchResults { get; set; }
}