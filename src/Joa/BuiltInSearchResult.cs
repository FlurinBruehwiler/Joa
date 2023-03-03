using JoaLauncher.Api;
using JoaLauncher.Api.Providers;

namespace Joa;

public class BuiltInSearchResult : SearchResult
{
    public required Action<IExecutionContext> ExecutionAction { get; set; }

    public override void Execute(IExecutionContext executionContext)
    {
        ExecutionAction(executionContext);
    }
}


