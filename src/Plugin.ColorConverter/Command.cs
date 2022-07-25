using JoaPluginsPackage.Plugin;

namespace ColorConverter;

public class Command : ICommand
{
    public string Caption { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Icon { get; init; } = null!;
}