using Interfaces;
using Interfaces.Plugin;

namespace ApplicationSearch;

public class Command : ICommand
{
    public string Caption { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Icon { get; set; } = null!;
}