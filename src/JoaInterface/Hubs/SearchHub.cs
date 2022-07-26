using JoaCore;
using JoaPluginsPackage.Logger;
using Microsoft.AspNetCore.SignalR;

namespace JoaInterface.Hubs;

public class SearchHub : Hub
{
    private readonly Search _search;
    public SearchHub(Search search)
    {
        _search = search;
    }
    
    public async Task<List<PluginCommand>> GetSearchResults(string searchString)
    {
        return await _search.GetSearchResults(searchString);
    }

    public async Task ExecuteSearchResult(string commandId, string actionKey)
    {
        if (!Guid.TryParse(commandId, out var guidId))
            return;
        await _search.ExecuteCommand(guidId, actionKey);
    }
}