using System.Diagnostics;
using Joa.Api;

namespace ApplicationSearch;

public class SearchResult : ISearchResult
{
    public string FilePath { get; init; } = default!;
    public string Title { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<ContextAction>? Actions { get; init; }
    public void Execute(IExecutionContext executionContext)
    {
        var info = new ProcessStartInfo ( FilePath )
        {
            UseShellExecute = true
        };
        Process.Start(info);
    }
}