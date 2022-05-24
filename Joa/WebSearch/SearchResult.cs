using Interfaces;

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

    public string Caption { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public SearchEngine SearchEngine { get; set; }
    public string SeachString { get; set; }
}