using FlurinBruehwiler.Helpers;
using JoaLauncher.Api;
using JoaLauncher.Api.Injectables;

namespace BookmarksSearch;

public class SerachResult : SearchResult
{
    public override void Execute(IExecutionContext executionContext)
    {
        var browserHelper = executionContext.ServiceProvider.GetService(typeof(IBrowserHelper)) as IBrowserHelper;

        browserHelper?.OpenWebsite(Description);
    }
}
