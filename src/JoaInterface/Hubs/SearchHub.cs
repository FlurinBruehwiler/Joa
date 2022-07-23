using JoaCore;
using JoaPluginsPackage.Logger;
using Microsoft.AspNetCore.SignalR;

namespace JoaInterface.Hubs;

public class SearchHub : Hub
{
    private readonly Search _search;
    public static IHubContext<SearchHub>? GlobalContext { get; private set; }
    public SearchHub(Search search, IHubContext<SearchHub> ctx)
    {
        _search = search;
        GlobalContext = ctx;
    }
    
    public async Task GetSearchResults(string searchString)
    {
        var searchResult = await _search.GetSearchResults(searchString);
        await Clients.Caller.SendAsync("ReceiveSearchResults",  searchResult);
    }

    public async Task ExecuteSearchResult(string commandId)
    {
        JoaLogger.GetInstance().Log(commandId, IJoaLogger.LogLevel.Info);
        if (!Guid.TryParse(commandId, out var guidId))
            return;
        await _search.ExecuteCommand(guidId);
    }
}