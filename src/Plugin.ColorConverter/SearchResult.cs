using JoaPluginsPackage.Plugin;

namespace ColorConverter;

public class SearchResult : ISearchResult
{
    public string Caption { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Icon { get; init; } = null!;
    public List<IContextAction> Actions { get; set; }
}