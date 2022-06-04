namespace JoaPluginsPackage.Plugin;

public interface ICommand
{
    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
}