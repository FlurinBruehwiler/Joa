using JoaPluginsPackage;
using JoaPluginsPackage.Providers;
using Newtonsoft.Json;

namespace WebSearch;

public class WebProvider : IProvider
{
    private readonly WebSearchSettings _settings;
    private readonly HttpClient _client;

    public WebProvider(WebSearchSettings settings, HttpClient client)
    {
        _settings = settings;
        _client = client;
    }

    public List<ISearchResult> GetSearchResults(string searchString)
    {
        var searchEngine = _settings.SearchEngines.FirstOrDefault(x =>
            searchString.StartsWith(x.Prefix));

        if (searchEngine == null || searchString.Length < searchEngine.Prefix.Length)
        {
            return new List<ISearchResult>();
        }
        
        searchString = searchString.Remove(0, searchEngine.Prefix.Length);
        
        var searchResults = new List<ISearchResult>
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
        {
            return searchResults;
        }

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
}