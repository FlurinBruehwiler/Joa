﻿using System.Diagnostics;
using Interfaces;
using Interfaces.Logger;
using Interfaces.Plugin;
using Interfaces.Settings;
using Interfaces.Settings.Attributes;
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
    public bool Validator(string searchString) =>
        SearchEngines.Any(x => searchString.StartsWith(x.Prefix));

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
        var searchEngine = SearchEngines.FirstOrDefault(x =>
            searchString.StartsWith(x.Prefix));

        if (searchEngine == null || searchString.Length < searchEngine.Prefix.Length)
            return new List<ISearchResult>();
        
        searchString = searchString.Remove(0, searchEngine.Prefix.Length);
        
        var client = new HttpClient();

        var httpResponse = client.GetAsync(searchEngine.SuggestionUrl
                .Replace("{{query}}",searchString))
            .GetAwaiter().GetResult();

        var searchResults = new List<ISearchResult>
        {
            new SearchResult(searchEngine.Name, $"Search on {searchEngine.Name} for \"{searchString}\"", "", searchEngine, searchString)
        };

        dynamic response = JsonConvert.DeserializeObject(httpResponse.Content.ReadAsStringAsync().GetAwaiter()
            .GetResult()) ?? throw new InvalidOperationException();

        List<string> suggestions = response[1].ToObject<List<string>>();
        
        searchResults.AddRange(suggestions.Select(suggestion 
                => new SearchResult(suggestion, 
                    $"Search on Google for \"{suggestion}\"", 
                    "", 
                    searchEngine, 
                    searchString))
            .ToList());
        
        return searchResults;
    }

    public void Execute(ISearchResult result)
    {
        if (result is not SearchResult searchResult) return;
        Process.Start("chrome.exe", 
            searchResult.SearchEngine.Url
                .Replace("{{query}}", searchResult.SeachString));
    }
}
