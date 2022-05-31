namespace Interfaces.Plugin;

public interface ICommand
{
    public string Caption { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}