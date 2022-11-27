using Joa.Step;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Joa.Hubs;

public class SearchHub : Hub
{
    private readonly JoaManager _joaManager;

    public SearchHub(JoaManager joaManager)
    {
        _joaManager = joaManager;
    }
    
    public Task UpdateSearchResults(string searchString)
    {
        return _joaManager.CurrentScope?.ServiceProvider.GetRequiredService<Search>().UpdateSearchResults(searchString) ?? Task.CompletedTask;
    }

    public void GoToStep(Guid stepId)
    {
        _joaManager.CurrentScope?.ServiceProvider.GetRequiredService<StepsManager>().GoToStep(stepId);
    }

    public List<DtoStep> GetSteps()
    {
        return _joaManager.CurrentScope?.ServiceProvider.GetRequiredService<StepsManager>().GetSteps().Select(x => new DtoStep
        {
            Id = x.Id,
            Name = x.Name
        }).ToList() ?? new List<DtoStep>();
    }

    public async Task ExecuteSearchResult(string commandId, string actionKey)
    {
        if (!Guid.TryParse(commandId, out var guidId))
            return;
        var task =_joaManager.CurrentScope?.ServiceProvider.GetRequiredService<Search>().ExecuteCommand(guidId, actionKey);
        if (task is not null)
            await task;
    }
}