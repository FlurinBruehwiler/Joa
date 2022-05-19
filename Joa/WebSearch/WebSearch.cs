using System.Diagnostics;
using Interfaces;
using Interfaces.Logger;
using Interfaces.Plugin;
using Interfaces.Settings;
using Newtonsoft.Json;

namespace WebSearch;

[Plugin("Web Search", "Lets you search on the web!")]
public class WebSearch : IPlugin
{
    private readonly IJoaLogger _logger;

    public WebSearch(IJoaLogger logger)
    {
        _logger = logger;
    }
    
    public bool AcceptNonMatchingSearchString => false;
    public List<Func<string, bool>> Matchers => new();
    
    [SettingProperty(Name = "Web Search Engines")]
    public List<SearchEngine> SearchEngines { get; set; } = new()
    {
        new SearchEngine
        {
            Name = "Google",
            Prefix = "g?",
            Url = "https://google.com/search?q={{query}}",
            SuggestionUrl = "https://www.google.com/complete/search?client=opera&q={{query}}",
            IconType = IconType.Svg,
            Icon = "https://google.com/favicon.ico",
            Priority = 2,
            Fallback = false,
            EncodeSearchTerm = true
        },
        new SearchEngine
        {
            Name = "Youtube",
            Prefix = "y?",
            Url = "https://www.youtube.com/results?search_query={{query}}",
            SuggestionUrl = "https://www.google.com/complete/search?ds=yt&output=firefox&q={{query}}",
            IconType = IconType.Svg,
            Icon = "https://www.youtube.com/favicon.ico",
            Priority = 5,
            Fallback = false,
            EncodeSearchTerm = true
        }
    };

    public List<ISearchResult> GetResults(string searchString)
    {
        throw new Exception();
        
        var client = new HttpClient();

        var httpResponse = client.GetAsync($"https://www.google.com/complete/search?client=opera&q={searchString}")
            .GetAwaiter().GetResult();

        var searchResults = new List<ISearchResult>
        {
            new SearchResult("Google", $"Search on Google for \"{searchString}\"", "")
        };

        dynamic response = JsonConvert.DeserializeObject(httpResponse.Content.ReadAsStringAsync().GetAwaiter()
            .GetResult()) ?? throw new InvalidOperationException();

        List<string> suggestions = response[1].ToObject<List<string>>();
        
        searchResults.AddRange(suggestions.Select(suggestion => new SearchResult(suggestion, $"Search on Google for \"{suggestion}\"", ""))
            .ToList());
        
        return searchResults;
    }

    public void Execute(ISearchResult result)
    {
        if (result is not SearchResult) return;
        Process.Start("chrome.exe", "https://www.google.ch");
    }
}
