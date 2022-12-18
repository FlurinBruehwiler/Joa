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
        return GetRequiredService<Search>().UpdateSearchResults(searchString);
    }

    public void GoToStep(Guid stepId)
    {
        GetRequiredService<StepsManager>().GoToStep(stepId);
    }

    public void UpdateSteps()
    {
        GetRequiredService<StepsManager>().UpdateStepsOnClient();
    }

    public async Task ExecuteSearchResult(string commandId, string actionId)
    {
        if (!Guid.TryParse(commandId, out var guidId))
            return;
        await GetRequiredService<Search>().ExecuteCommand(guidId, actionId);
    }

    private T GetRequiredService<T>()
    {
        return (T)_joaManager.CurrentScope?.ServiceProvider.GetRequiredService(typeof(T))! ?? throw new InvalidOperationException();
    }
}