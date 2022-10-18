using JoaPluginsPackage;
using JoaPluginsPackage.Injectables;

namespace BookmarksSearch;

public class BookmarkSerachResult : ISearchResult
{
    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<ContextAction>? Actions { get; init; }
    public ISearchResultProvider? Execute(IExecutionContext executionContext)
    {
        var browserHelper = executionContext.ServiceProvider.GetService(typeof(IBrowserHelper)) as IBrowserHelper;

        browserHelper?.OpenWebsite(Description);

        return null;
    }
}