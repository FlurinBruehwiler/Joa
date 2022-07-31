using JoaPluginsPackage.Plugin;

namespace ApplicationSearch;

public class SearchResult : ISearchResult
{
    public SearchResult(string caption, string description, string icon, List<IContextAction> actions, string filePath)
    {
        Caption = caption;
        Description = description;
        Icon = icon;
        Actions = actions;
        FilePath = filePath;
    }

    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<IContextAction> Actions { get; set; }
    public string FilePath { get; set; }
}