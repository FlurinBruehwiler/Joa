using JoaPluginsPackage;
using JoaPluginsPackage.Logger;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Plugin.Search;
using JoaPluginsPackage.Settings.Attributes;
using Newtonsoft.Json;

namespace WebSearch;

[Plugin("Web Search", "Lets you search on the web!", "", "", "")]
public class WebSearch : IStrictSearchPlugin
{
    private readonly IJoaLogger _logger;
    private readonly IBrowserHelper _browserHelper;
    private readonly HttpClient _client = new();
    
    public WebSearch(IJoaLogger logger, IBrowserHelper browserHelper)
    {
        _logger = logger;
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

    public List<ISearchResult> GetStrictSearchResults(string searchString)
    {
        var searchEngine = SearchEngines.FirstOrDefault(x =>
            searchString.StartsWith(x.Prefix));

        if (searchEngine == null || searchString.Length < searchEngine.Prefix.Length)
            return new List<ISearchResult>();
        
        searchString = searchString.Remove(0, searchEngine.Prefix.Length);
        
        var searchResults = new List<ISearchResult>
        {
            new SearchResult(searchEngine.Name, $"Search on {searchEngine.Name} for \"{searchString}\"", "", searchEngine, searchString)
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
                => new SearchResult(suggestion, 
                    $"Search on Google for \"{suggestion}\"", 
                    "", 
                    searchEngine, 
                    suggestion.Replace(" ", "+")))
            .ToList());
        
        return searchResults;
    }

    public void Execute(ISearchResult result, IContextAction contextAction)
    {
        _logger.Log("execute", IJoaLogger.LogLevel.Info);
        if (result is not SearchResult searchResult) return;
        _browserHelper.OpenWebsite(searchResult.SearchEngine.Url
            .Replace("{{query}}", searchResult.SeachString));
    }
}
