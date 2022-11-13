using Joa.Api;

namespace JoaInterface.Step;

public class ExecutionContext : IExecutionContext
{
    private readonly ISearchResult _pluginSearchResult;

    public ExecutionContext(ISearchResult pluginSearchResult)
    {
        _pluginSearchResult = pluginSearchResult;
    }
    
    public required ContextAction ContextAction { get; set; }
    public required IServiceProvider ServiceProvider { get; set; }
    public StepBuilder? StepBuilder { get; set; }
    public IStepBuilder AddStepBuilder()
    {
        StepBuilder = new StepBuilder(ServiceProvider, _pluginSearchResult);
        return StepBuilder;
    }
}