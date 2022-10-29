using Github.Providers;
using Github.Providers.Contexts;
using JoaPluginsPackage;

namespace Github.SearchResults;

public class RepositorySearchResult : ISearchResult
{
    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<ContextAction>? Actions { get; init; }
    public void Execute(IExecutionContext executionContext)
    {
        executionContext.AddStepBuilder().AddProvider<RepositoryProvider>(new RepositoriesProviderContext());
    }
}