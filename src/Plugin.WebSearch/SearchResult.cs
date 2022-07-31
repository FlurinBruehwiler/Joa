using JoaPluginsPackage.Plugin;

namespace WebSearch;

public class SearchResult : ISearchResult
{
    public SearchResult(string caption, string description, string icon, SearchEngine searchEngine, string seachString)
    {
        Caption = caption;
        Description = description;
        Icon = icon;
        SearchEngine = searchEngine;
        SeachString = seachString;
    }

    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<IContextAction> Actions { get; set; }
    public SearchEngine SearchEngine { get; set; }
    public string SeachString { get; set; }
}