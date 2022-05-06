using Interfaces;
using Interfaces.Settings;
using Newtonsoft.Json;

namespace HelloPlugin
{
    public class WebSearch : IPlugin
    {
        private readonly ISettings _settings;

        public WebSearch(ISettings settings)
        {
            _settings = settings;
        }
        
        public string Name => "Web Search";
        public string Description => "Displays hello message.";
        public bool AcceptNonMatchingSearchString => false;
        public List<Func<string, bool>> Matchers => new();
        public PluginSetting PluginSetting => new PluginSetting(new List<Setting>
        {
            new SettingList("Web Search Engines",
                "engines",
                new List<Setting>
                {
                    new SettingText("Name", "name", ""),
                    new SettingText("Prefix", "prefix", ""),
                    new SettingText("URL", "url",""),
                    new SettingText("Suggestion URL", "surl", ""),
                    new SettingSelection<IconType>("Icon type", "icontype",IconType.SVG),
                    new SettingText("Icon", "icon",""),
                    new SettingText("Priority", "priority",""),
                    new SettingToggle("Fallback","fallback", false),
                    new SettingToggle("Encode search term", "encode",true)
                },
                "Add new websearch engine")
        });
        

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

        public void Execute(ISearchResult searchResult)
        {
            throw new NotImplementedException();
        }
    }
}