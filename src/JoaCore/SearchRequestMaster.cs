using Microsoft.AspNetCore.SignalR;

namespace JoaCore;

public class SearchRequestMaster
{
    public event EventHandler<IncomingSearchRequestEventArgs>? IncomingSearchRequest;

    public SearchRequestMaster(Search search){
        IncomingSearchRequest += async (sender, args) =>
        {
            var res = await search.GetSearchResults(args.SearchString);
            await args.Clients.All.SendAsync("ReceiveSearchResults", args.SearchString, res);
        };
    }
    
    public void OnIncomingSearchRequest(string searchString, IHubCallerClients caller)
    {
        IncomingSearchRequest?.Invoke(this, new IncomingSearchRequestEventArgs(searchString, caller));
    }
}

public class IncomingSearchRequestEventArgs
{
    public IncomingSearchRequestEventArgs(string searchString, IHubCallerClients clients)
    {
        SearchString = searchString;
        Clients = clients;
    }

    public string SearchString { get; set; }
    public IHubCallerClients Clients { get; set; }
}