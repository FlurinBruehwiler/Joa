using JoaPluginsPackage;
using JoaPluginsPackage.Plugin;

namespace WebSearch;

public class WebSearchResult : SearchResult
{
    public WebSearchResult(string caption, string description, string icon, string url) : base(caption, description, icon)
    {
        Url = url;
    }

    public string Url { get; set; }
}