using Interfaces.Plugin;

namespace JoaCore.PluginCore;

public class PluginCommand
{
    public PluginCommand(ICommand command, Guid pluginId)
    {
        Command = command;
        PluginId = pluginId;
    }

    public ICommand Command { get; set; }
    public Guid PluginId { get; set; }
}