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
    
    public Task GetSearchResults(string searchString)
    {
        _search.UpdateSearchResults(searchString, results =>
        {
            Clients.Caller.SendAsync("ReceiveSearchResults", searchString, results);
        });
        return Task.CompletedTask;
    }

    public async Task ExecuteSearchResult(string commandId, string actionKey)
    {
        if (!Guid.TryParse(commandId, out var guidId))
            return;
        await _search.ExecuteCommand(guidId, actionKey);
    }
}