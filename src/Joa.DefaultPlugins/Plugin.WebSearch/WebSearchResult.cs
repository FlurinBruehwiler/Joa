using FlurinBruehwiler.Helpers;
using JoaLauncher.Api;
using JoaLauncher.Api.Injectables;

namespace WebSearch;

public class WebSearchResult : SearchResult
{
    public required string Url { get; init; }

    public override void Execute(IExecutionContext executionContext)
    {
        var browserHelper = executionContext.ServiceProvider.GetService(typeof(IBrowserHelper)) as IBrowserHelper;

        browserHelper?.OpenWebsite(Url);
    }
}