namespace Joa.Api;

/// <summary>
/// A parameter passed in to the
/// <see cref="ISearchResult.Execute"/> Method on a SearchResult
/// Containt information about the context in which the execution
/// happens
/// </summary>
public interface IExecutionContext
{
    /// <summary>
    /// Is the <see cref="ContextAction"/> that the user selected when
    /// executing the <see cref="ISearchResult"/>
    /// </summary>
    public ContextAction ContextAction { get; set; }
    
    /// <summary>
    /// A ServiceProvider that can be used to inject services
    /// </summary>
    public IServiceProvider ServiceProvider { get; set; }
    
    /// <summary>
    /// Creates a new StepBuilder and returns it.
    /// If this method is called, the UI will navigate
    /// to the new Step
    /// </summary>
    /// <returns></returns>
    public IStepBuilder AddStepBuilder();
}