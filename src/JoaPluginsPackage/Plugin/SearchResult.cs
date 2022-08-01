namespace JoaPluginsPackage.Plugin;

public class SearchResult
{
    public SearchResult(string caption, string description, string icon, List<ContextAction>? actions = null)
    {
        Caption = caption;
        Description = description;
        Icon = icon;
        Actions = actions ?? new List<ContextAction>
        {
            new("enter", "Open", null)
        };
    }

    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<ContextAction> Actions { get; set; }
}