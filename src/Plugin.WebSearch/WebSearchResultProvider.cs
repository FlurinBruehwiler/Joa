using JoaPluginsPackage;
using JoaPluginsPackage.Enums;
using Newtonsoft.Json;

namespace WebSearch;

public class WebSearchResultProvider : ISearchResultProvider
{
    private readonly WebSearchSettings _settings;
    private readonly HttpClient _client;

    public WebSearchResultProvider(WebSearchSettings settings, HttpClient client)
    {
        _settings = settings;
        _client = client;
    }

    public List<ISearchResult> SearchResults { get; set; } = new();
    public SearchResultLifetime SearchResultLifetime { get; set; } = SearchResultLifetime.Search; 
    public void UpdateSearchResults(ISearchProviderContext searchProviderContext)
    {
        var searchEngine = _settings.SearchEngines.FirstOrDefault(x =>
            searchProviderContext.SearchString.StartsWith(x.Prefix));

        if (searchEngine == null || searchProviderContext.SearchString.Length < searchEngine.Prefix.Length)
        {
            SearchResults = new List<ISearchResult>();
            return;
        }
        
        searchProviderContext.SearchString = searchProviderContext.SearchString.Remove(0, searchEngine.Prefix.Length);
        
        var searchResults = new List<ISearchResult>
        {
            new WebSearchResult
            {
                Caption = searchEngine.Name,
                Description = $"Search on {searchEngine.Name} for \"{searchProviderContext.SearchString}\"",
                Icon = "",
                Url = ""
            }
        };

        if (string.IsNullOrEmpty(searchProviderContext.SearchString))
        {
            SearchResults = searchResults;
            return;
        }

        var httpResponse = _client.GetAsync(searchEngine.SuggestionUrl
                .Replace("{{query}}",searchProviderContext.SearchString))
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
        
        SearchResults = searchResults;
    }
}