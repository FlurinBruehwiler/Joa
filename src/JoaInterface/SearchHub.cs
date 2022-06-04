using JoaCore;
using Microsoft.AspNetCore.SignalR;

namespace JoaInterface;

public class SearchHub : Hub
{
    private readonly Search _search;

    public SearchHub(Search search)
    {
        _search = search;
    }
    
    public async Task GetSearchResults(string searchString)
    {
        var searchResult = await _search.GetSearchResults(searchString);
        await Clients.Caller.SendAsync("ReceiveSearchResults",  searchResult);
    }

    public async Task ExecuteSearchResult(string commandId)
    {
        if (!Guid.TryParse(commandId, out var guidId))
            return;
        await _search.ExecuteCommand(guidId);
    }
}