using ICommand = JoaPluginsPackage.Plugin.ICommand;

namespace JoaCore;

public class PluginCommand
{
    public PluginCommand(ICommand command, Guid pluginId)
    {
        Command = command;
        PluginId = pluginId;
        CommandId = Guid.NewGuid();
    }

    public ICommand Command { get; }
    public Guid PluginId { get; }
    public Guid CommandId { get; set; }
}