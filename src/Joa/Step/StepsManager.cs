using Joa.Hubs;
using Joa.PluginCore;
using Microsoft.AspNetCore.SignalR;

namespace Joa.Step;

public class StepsManager
{
    private readonly IHubContext<SearchHub> _hubContext;
    private readonly Stack<Step> _steps;

    public StepsManager(PluginManager pluginManager, IHubContext<SearchHub> hubContext)
    {
        _hubContext = hubContext;
        _steps = new Stack<Step>();
        AddStep(new Step
        {
            Providers = pluginManager.GlobalProviders,
            Name = "Global Step"
        });
    }

    public List<Step> GetSteps()
    {
        return _steps.ToList();
    }

    public Step GetCurrentStep()
    {
        return _steps.Peek();
    }

    public void AddStep(Step step)
    {
        _steps.Push(step);
        _hubContext.Clients.All.SendAsync("AddStep", new DtoStep { Id = step.Id, Name = step.Name });
    }
    
    public void GoToStep(Guid stepId)
    {
        while (GetCurrentStep().Id != stepId && _steps.Count > 1)
        {
            _steps.Pop();
        }
    }
}