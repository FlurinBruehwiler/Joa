using JoaPluginsPackage;

namespace BookmarksSearch;

public class BookmarkSerachResult : ISearchResult
{
    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<ContextAction>? Actions { get; init; }
    public void Execute(ContextAction action)
    {
        throw new NotImplementedException();
    }
}