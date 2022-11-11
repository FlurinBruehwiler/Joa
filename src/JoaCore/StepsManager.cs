namespace JoaCore;

public class StepsManager
{
    private readonly Stack<Step> _steps;

    public StepsManager(PluginManager pluginManager)
    {
        _steps = new Stack<Step>();
        AddStep(new Step
        {
            Providers = pluginManager.GlobalProviders
        });
    }

    public Step GetCurrentStep()
    {
        return _steps.Peek();
    }

    public void AddStep(Step step)
    {
        _steps.Push(step);
    }
    
    public void GoToStep(Guid stepId)
    {
        while (GetCurrentStep().StepId != stepId && _steps.Count > 1)
        {
            _steps.Pop();
        }
    }
}