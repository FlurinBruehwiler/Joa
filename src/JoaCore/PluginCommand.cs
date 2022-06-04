using ICommand = JoaPluginsPackage.Plugin.ICommand;

namespace JoaCore;

public class PluginCommand
{
    public PluginCommand(ICommand command, Guid pluginId)
    {
        Command = command;
        PluginId = pluginId;
    }

    public ICommand Command { get; }
    public Guid PluginId { get; }
}