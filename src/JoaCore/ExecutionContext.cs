using JoaPluginsPackage;

namespace JoaCore;

public class ExecutionContext : IExecutionContext
{
    public required ContextAction ContextAction { get; set; }
    public required IServiceProvider ServiceProvider { get; set; }
    public StepBuilder? StepBuilder { get; set; }
    public IStepBuilder AddStepBuilder()
    {
        StepBuilder = new StepBuilder(ServiceProvider);
        return StepBuilder;
    }
}