using Joa.Api;
using Joa.Api.Injectables;

namespace WebSearch;

public class WebSearchResult : ISearchResult
{
    public string Url { get; init; } = default!;
    public string Title { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<ContextAction>? Actions { get; init; }
    public void Execute(IExecutionContext executionContext)
    {
        var browserHelper = executionContext.ServiceProvider.GetService(typeof(IBrowserHelper)) as IBrowserHelper;

        browserHelper?.OpenWebsite(Url);
    }
}