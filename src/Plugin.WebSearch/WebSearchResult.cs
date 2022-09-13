using JoaPluginsPackage;
using JoaPluginsPackage.Plugin;

namespace WebSearch;

public class WebSearchResult : ISearchResult
{
    public string Url { get; init; } = default!;
    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<ContextAction>? Actions { get; init; }
    public void Execute(ContextAction action)
    {
        throw new NotImplementedException();
    }
}