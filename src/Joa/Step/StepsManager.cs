using JoaInterface.Hubs;
using JoaInterface.PluginCore;
using Joa.Api.Injectables;
using Microsoft.AspNetCore.SignalR;

namespace JoaInterface.Step;

public class StepsManager
{
    private readonly IHubContext<SearchHub> _hubContext;
    private readonly Stack<Step> _steps;

    public StepsManager(PluginManager pluginManager, IJoaLogger logger, IHubContext<SearchHub> hubContext)
    {
        _hubContext = hubContext;
        logger.Info(nameof(StepsManager));
        _steps = new Stack<Step>();
        AddStep(new Step
        {
            Providers = pluginManager.GlobalProviders,
            Name = string.Empty
        });
    }

    public Step GetCurrentStep()
    {
        return _steps.Peek();
    }

    public void AddStep(Step step)
    {
        _steps.Push(step);
        _hubContext.Clients.All.SendAsync("AddStep", new { id = step.Id, name = "some name" });
    }
    
    public void GoToStep(Guid stepId)
    {
        while (GetCurrentStep().Id != stepId && _steps.Count > 1)
        {
            _steps.Pop();
        }
    }
}