using System.Diagnostics;
using Interfaces;
using Interfaces.Settings;
using Newtonsoft.Json;

namespace WebSearch;

[Plugin("Web Search", "Lets you search on the web!")]
public class WebSearch : IPlugin
{
    private readonly IJoaSettings _joaSettings;

    public WebSearch(IJoaSettings joaSettings)
    {
        _joaSettings = joaSettings;
        ID = new Guid();
    }

    public Guid ID { get; }
    public bool AcceptNonMatchingSearchString => false;
    public List<Func<string, bool>> Matchers => new();

    [SettingProperty(Name = "Web Search Engines")]
    public List<SearchEngine> SearchEngines { get; set; } = new List<SearchEngine>
    {
        new SearchEngine
        {
            Name = "Google",
            Prefix = "g?",
            Url = "https://google.com/search?q={{query}}",
            SuggestionUrl = "https://www.google.com/complete/search?client=opera&q={{query}}",
            IconType = IconType.SVG,
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
            IconType = IconType.SVG,
            Icon = "https://www.youtube.com/favicon.ico",
            Priority = 5,
            Fallback = false,
            EncodeSearchTerm = true
        }
    };
    
    public IEnumerable<ISearchResult> GetResults(string searchString)
    {
        var client = new HttpClient();

        HttpResponseMessage? httpResponse = null;
        
        try
        {
            httpResponse = client.GetAsync($"https://www.google.com/complete/search?client=opera&q={searchString}")
                .GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        var searchResults = new List<ISearchResult>
        {
            new SearchResult("Google", $"Search on Google for \"{searchString}\"", "")
        };

        if (httpResponse != null)
        {
            try
            {
                dynamic response = JsonConvert.DeserializeObject(httpResponse.Content.ReadAsStringAsync().GetAwaiter()
                    .GetResult()) ?? throw new InvalidOperationException();

                List<string> suggestions = response[1].ToObject<List<string>>();
                
                searchResults.AddRange(suggestions.Select(suggestion => new SearchResult(suggestion, $"Search on Google for \"{suggestion}\"", ""))
                    .ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return searchResults;
    }

    public void Execute(ISearchResult result)
    {
        if (result is not  SearchResult sr) return;
        Process.Start("chrome.exe", "https://www.google.ch");
    }
}

public class SearchEngine
{
    [SettingProperty] public string Name { get; set; } = string.Empty;

    [SettingProperty] public string Prefix { get; set; } = string.Empty;

    [SettingProperty(Name = "URL")] public string Url { get; set; } = string.Empty;

    [SettingProperty(Name = "Suggestion URL")]
    public string SuggestionUrl { get; set; } = string.Empty;

    [SettingProperty] public IconType IconType { get; set; } = IconType.SVG;
    
    [SettingProperty] public string Icon { get; set; } = string.Empty;

    [SettingProperty] public int Priority { get; set; } = 0;

    [SettingProperty] public bool Fallback { get; set; } = false;

    [SettingProperty] public bool EncodeSearchTerm { get; set; } = true;
}
