using JoaCore;
using JoaInterface.HotKey;
using Microsoft.AspNetCore.SignalR;

namespace JoaInterface;

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
        if (!Guid.TryParse(commandId, out var guidId))
            return;
        await _search.ExecuteCommand(guidId);
    }
}