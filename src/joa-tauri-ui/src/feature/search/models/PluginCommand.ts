import Command from "./Command";

export default interface PluginCommand {
    pluginId : string,
    commandId: string,
    command: Command,
}
