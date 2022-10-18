using JoaPluginsPackage;

namespace Github;

public class RepositorySearchResult : ISearchResult
{
    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<ContextAction>? Actions { get; init; }
    public ISearchResultProvider? Execute(IExecutionContext executionContext)
    {
        throw new NotImplementedException();
    }
}