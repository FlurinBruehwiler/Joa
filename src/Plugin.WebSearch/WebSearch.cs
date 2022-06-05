using System.Diagnostics;
using System.Runtime.InteropServices;
using JoaPluginsPackage.Logger;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Settings.Attributes;
using Newtonsoft.Json;

namespace WebSearch;

[Plugin("Web Search", "Lets you search on the web!")]
public class WebSearch : IPlugin, IStrictPlugin
{
    private readonly IJoaLogger _logger;
    private readonly HttpClient _client = new();
    
    public WebSearch(IJoaLogger logger)
    {
        _logger = logger;
    }
    
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

    public List<ICommand> GetResults(string searchString)
    {
        var searchEngine = SearchEngines.FirstOrDefault(x =>
            searchString.StartsWith(x.Prefix));

        if (searchEngine == null || searchString.Length < searchEngine.Prefix.Length)
            return new List<ICommand>();
        
        searchString = searchString.Remove(0, searchEngine.Prefix.Length);
        
        var searchResults = new List<ICommand>
        {
            new Command(searchEngine.Name, $"Search on {searchEngine.Name} for \"{searchString}\"", "", searchEngine, searchString)
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
                => new Command(suggestion, 
                    $"Search on Google for \"{suggestion}\"", 
                    "", 
                    searchEngine, 
                    suggestion))
            .ToList());
        
        return searchResults;
    }

    public void Execute(ICommand result)
    {
        if (result is not Command searchResult) return;
        OpenBrowser(searchResult.SearchEngine.Url
            .Replace("{{query}}", searchResult.SeachString));
    }
    
    public static void OpenBrowser(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}
