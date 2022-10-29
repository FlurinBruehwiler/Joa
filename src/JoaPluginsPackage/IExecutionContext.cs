namespace JoaPluginsPackage;

public interface IExecutionContext
{
    public ContextAction ContextAction { get; set; }
    public IServiceProvider ServiceProvider { get; set; }
    public IStepBuilder AddStepBuilder();
}