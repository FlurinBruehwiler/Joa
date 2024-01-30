using System.Text.Json;
using System.Web;
using JoaLauncher.Api;
using JoaLauncher.Api.Providers;

namespace WebSearch;

public class WebProvider : IProvider
{
    private readonly WebSearchSettings _settings;
    private readonly HttpClient _client;

    public WebProvider(WebSearchSettings settings)
    {
        _settings = settings;
        _client = new HttpClient();
    }

    public List<SearchResult> GetSearchResults(string searchString)
    {
        var searchEngine = _settings.SearchEngines.FirstOrDefault(x =>
            searchString.StartsWith(x.Prefix));

        if (searchEngine == null || searchString.Length < searchEngine.Prefix.Length)
        {
            return new List<SearchResult>();
        }

        searchString = searchString.Remove(0, searchEngine.Prefix.Length);

        var searchResults = new List<SearchResult>
        {
            new WebSearchResult
            {
                Title = searchEngine.Name,
                Description = $"Search hallo on {searchEngine.Name} for \"{searchString}\"",
                Icon = "",
                Url = searchEngine.Url.Replace("{{query}}", HttpUtility.UrlEncode(searchString))
            }
        };

        if (string.IsNullOrEmpty(searchString))
        {
            return searchResults;
        }

        var httpResponse = _client.GetAsync(searchEngine.SuggestionUrl
                .Replace("{{query}}", searchString))
            .GetAwaiter().GetResult();

        var jsonDocument = JsonDocument.Parse(httpResponse.Content.ReadAsStringAsync().GetAwaiter()
            .GetResult());

        var suggestions = jsonDocument.RootElement[1].EnumerateArray().Select(x => x.ToString()).ToList();

        searchResults.AddRange(suggestions.Select(suggestion
                => new WebSearchResult
                {
                    Title = suggestion,
                    Description = $"Search on Google for \"{suggestion}\"",
                    Icon = "",
                    Url = searchEngine.Url.Replace("{{query}}", HttpUtility.UrlEncode(suggestion))
                })
            .ToList());

        return searchResults;
    }
}