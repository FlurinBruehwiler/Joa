using JoaCore;
using Microsoft.AspNetCore.SignalR;

namespace JoaInterface.Hubs;

public class SearchHub : Hub
{
    private readonly Search _search;

    public SearchHub(Search search)
    {
        _search = search;
    }
    
    public Task GetSearchResults(string searchString, Guid stepId)
    {
        return _search.UpdateSearchResults(searchString, stepId, results =>
        {
            Clients.Caller.SendAsync("ReceiveSearchResults", searchString, results);
        });
    }

    public async Task ExecuteSearchResult(string commandId, string actionKey)
    {
        if (!Guid.TryParse(commandId, out var guidId))
            return;
        await _search.ExecuteCommand(guidId, actionKey);
    }
}