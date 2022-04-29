using PluginBase;

namespace HelloPlugin;

public class SearchResult : ISearchResult
{
    public SearchResult(string title, string description, string icon)
    {
        Title = title;
        Description = description;
        Icon = icon;
    }

    public string Title { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}