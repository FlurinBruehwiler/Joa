using System.Reflection;
using JoaCore;
using Microsoft.AspNetCore.SignalR;

namespace JoaInterface.Hubs;

public class SearchHub : Hub
{
    private readonly Search _search;
    private readonly SearchRequestMaster _searchRequestMaster;

    public SearchHub(Search search, SearchRequestMaster searchRequestMaster)
    {
        _search = search;
        _searchRequestMaster = searchRequestMaster;
    }
    
    public Task GetSearchResults(string searchString)
    {
        _searchRequestMaster.OnIncomingSearchRequest(searchString, Clients);
        return Task.CompletedTask;
    }

    public async Task ExecuteSearchResult(string commandId, string actionKey)
    {
        if (!Guid.TryParse(commandId, out var guidId))
            return;
        await _search.ExecuteCommand(guidId, actionKey);
    }
}