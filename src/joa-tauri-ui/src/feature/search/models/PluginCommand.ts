import Command from "./Command";

export default interface PluginCommand {
    PluginId : string,
    CommandId: string,
    Command: Command,
}
