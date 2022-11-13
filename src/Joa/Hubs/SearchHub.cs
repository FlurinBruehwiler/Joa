using JoaInterface.Step;
using Microsoft.AspNetCore.SignalR;

namespace JoaInterface.Hubs;

public class SearchHub : Hub
{
    private readonly Search _search;
    private readonly StepsManager _stepsManager;

    public SearchHub(Search search, StepsManager stepsManager)
    {
        _search = search;
        _stepsManager = stepsManager;
    }
    
    public Task GetSearchResults(string searchString)
    {
        return _search.UpdateSearchResults(searchString);
    }

    public void GoToStep(Guid stepId)
    {
        _stepsManager.GoToStep(stepId);
    }

    public async Task ExecuteSearchResult(string commandId, string actionKey)
    {
        if (!Guid.TryParse(commandId, out var guidId))
            return;
        await _search.ExecuteCommand(guidId, actionKey);
    }
}