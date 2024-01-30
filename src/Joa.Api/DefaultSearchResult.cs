namespace JoaLauncher.Api;

/// <inheritdoc />
public sealed class DefaultSearchResult : SearchResult
{
    /// <summary>
    /// The Action which gets called if the user executes the search result
    /// </summary>
    public required Action<IExecutionContext> ExecutionAction { get; set; }
    
    public override void Execute(IExecutionContext executionContext)
    {
        ExecutionAction(executionContext);
    }
}