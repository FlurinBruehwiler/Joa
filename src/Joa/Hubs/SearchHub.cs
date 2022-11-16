using JoaInterface.Step;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace JoaInterface.Hubs;

public class SearchHub : Hub
{
    private readonly JoaManager _joaManager;

    public SearchHub(JoaManager joaManager)
    {
        _joaManager = joaManager;
    }
    
    public Task GetSearchResults(string searchString)
    {
        return _joaManager.CurrentScope.ServiceProvider.GetRequiredService<Search>().UpdateSearchResults(searchString);
    }

    public void GoToStep(Guid stepId)
    {
        _joaManager.CurrentScope.ServiceProvider.GetRequiredService<StepsManager>().GoToStep(stepId);
    }

    public async Task ExecuteSearchResult(string commandId, string actionKey)
    {
        if (!Guid.TryParse(commandId, out var guidId))
            return;
        await _joaManager.CurrentScope.ServiceProvider.GetRequiredService<Search>().ExecuteCommand(guidId, actionKey);
    }
}