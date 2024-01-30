using Github.Providers;
using JoaLauncher.Api;

namespace Github.SearchResults;

public class RepositoriesSearchResult : SearchResult
{
    public override void Execute(IExecutionContext executionContext)
    {
        executionContext.AddStepBuilder().AddProvider<RepositoriesProvider>();
    }
}