using JoaLauncher.Api;

namespace Joa.BuiltInPlugin;

public class BuiltInSearchResult : SearchResult
{
    public required Action<IExecutionContext> ExecutionAction { get; set; }

    public override void Execute(IExecutionContext executionContext)
    {
        ExecutionAction(executionContext);
    }
}


