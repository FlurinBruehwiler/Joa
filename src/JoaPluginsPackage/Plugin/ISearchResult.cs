namespace JoaPluginsPackage.Plugin;

public interface ISearchResult
{
    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<IAction> Actions { get; set; }
}