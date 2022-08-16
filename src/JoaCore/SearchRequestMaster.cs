using Microsoft.AspNetCore.SignalR;

namespace JoaCore;

public class SearchRequestMaster
{
    public event EventHandler<IncomingSearchRequestEventArgs>? IncomingSearchRequest;

    public SearchRequestMaster(JoaSearch joaSearch){
        IncomingSearchRequest += async (sender, args) =>
        {
            var res = await joaSearch.GetSearchResults(args.SearchString);
        };
    }
    
    public void OnIncomingSearchRequest(string searchString)
    {
        IncomingSearchRequest?.Invoke(this, new IncomingSearchRequestEventArgs(searchString));
    }
}

public class IncomingSearchRequestEventArgs
{
    public IncomingSearchRequestEventArgs(string searchString)
    {
        SearchString = searchString;
    }

    public string SearchString { get; set; }
}