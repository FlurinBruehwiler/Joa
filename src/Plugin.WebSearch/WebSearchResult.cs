using JoaPluginsPackage;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;

namespace WebSearch;

public class WebSearchResult : ISearchResult
{
    public string Url { get; init; } = default!;
    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<ContextAction>? Actions { get; init; }
    public List<ISearchResult>? Execute(IExecutionContext executionContext)
    {
        var browserHelper = executionContext.ServiceProvider.GetService(typeof(IBrowserHelper)) as IBrowserHelper;

        browserHelper?.OpenWebsite(Url);

        return null;
    }
}