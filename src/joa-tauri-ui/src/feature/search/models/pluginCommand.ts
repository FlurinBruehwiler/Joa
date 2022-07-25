import Command from "./command";

export default interface PluginCommand {
    pluginId : string,
    commandId: string,
    command: Command,
}
