using JoaPluginsPackage;

namespace Github;

public class RepositoriesSearchResult : ISearchResult
{
    public string Caption { get; init; } = "Github Repos";
    public string Description { get; init; } = "Search for Github Repos";
    public string Icon { get; init; }
    public List<ContextAction>? Actions { get; init; }
    public List<ISearchResult> Execute(IExecutionContext executionContext)
    {
        return new List<ISearchResult>
        {
            new RepositorySearchResult
            {
                Caption = "Joa",
                Description = "Joa the best",
                Icon = ""
            },
            new RepositorySearchResult
            {
                Caption = "2",
                Description = "Joa the best",
                Icon = ""
            }
        };
    }
}