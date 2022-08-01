using JoaPluginsPackage.Plugin;

namespace BookmarksSearch;

public class SearchResult : ISearchResult
{
    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<IContextAction> Actions { get; set; }
}