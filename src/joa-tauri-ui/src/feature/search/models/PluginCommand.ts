import Command from "./command";

export default interface PluginCommand {
    PluginId : string,
    CommandId: string,
    Command: Command,
}
