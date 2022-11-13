using Joa.Api;
using Joa.Api.Attributes;

namespace WebSearch;

public class WebSearchSettings : ISetting
{
    [SettingProperty(Name = "Web Search Engines")]
    public List<SearchEngine> SearchEngines { get; set; } = new()
    {
        DefaultSearchEngines.Google,
        DefaultSearchEngines.Youtube
    };
}