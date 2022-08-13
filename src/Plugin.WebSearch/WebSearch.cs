using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;
using Newtonsoft.Json;

namespace WebSearch;

[Plugin("Web Search", "Lets you search on the web!", "", "", "")]
public class WebSearch : IStrictSearchPlugin
{
    private readonly IBrowserHelper _browserHelper;
    private readonly HttpClient _client = new();
    
    public WebSearch(IBrowserHelper browserHelper)
    {
        _browserHelper = browserHelper;
    }
    
    public bool Validator(string searchString) =>
        SearchEngines.Any(x => searchString.StartsWith(x.Prefix));

    [SettingProperty(Name = "Web Search Engines")]
    public List<SearchEngine> SearchEngines { get; set; } = new()
    {
        DefaultSearchEngines.Google,
        DefaultSearchEngines.Youtube
    };

    public List<SearchResult> GetStrictSearchResults(string searchString)
    {
        var searchEngine = SearchEngines.FirstOrDefault(x =>
            searchString.StartsWith(x.Prefix));

        if (searchEngine == null || searchString.Length < searchEngine.Prefix.Length)
            return new List<SearchResult>();
        
        searchString = searchString.Remove(0, searchEngine.Prefix.Length);
        
        var searchResults = new List<SearchResult>
        {
            new WebSearchResult
            {
                Caption = searchEngine.Name,
                Description = $"Search on {searchEngine.Name} for \"{searchString}\"",
                Icon = "",
                Url = ""
            }
        };

        if (string.IsNullOrEmpty(searchString))
            return searchResults;

        var httpResponse = _client.GetAsync(searchEngine.SuggestionUrl
                .Replace("{{query}}",searchString))
            .GetAwaiter().GetResult();

        dynamic response = JsonConvert.DeserializeObject(httpResponse.Content.ReadAsStringAsync().GetAwaiter()
            .GetResult()) ?? throw new InvalidOperationException();

        List<string> suggestions = response[1].ToObject<List<string>>();
        
        searchResults.AddRange(suggestions.Select(suggestion 
                => new WebSearchResult
                {
                    Caption = suggestion,
                    Description = $"Search on Google for \"{suggestion}\"",
                    Icon = "",
                    Url = searchEngine.Url.Replace("{{query}}", suggestion)
                })
            .ToList());
        
        return searchResults;
    }

    public void Execute(SearchResult result, ContextAction contextAction)
    {
        if (result is not WebSearchResult searchResult) 
            return;
        _browserHelper.OpenWebsite(searchResult.Url);
    }
}
