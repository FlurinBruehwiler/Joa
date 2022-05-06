using Interfaces;

namespace HelloPlugin;

public class SearchResult : ISearchResult
{
    public SearchResult(string caption, string description, string icon)
    {
        Caption = caption;
        Description = description;
        Icon = icon;
    }

    public string Caption { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}