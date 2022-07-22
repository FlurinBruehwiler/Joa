import {HubConnection, HubConnectionBuilder, HubConnectionState} from "@microsoft/signalr";
import {useState} from "react";
import PluginCommand from "../models/PluginCommand";
import {executeCommandMethod, receiveCommandsMethod, updateCommandsMethod} from "../models/JoaMethods";

//fds
export function useJoaSearch() : [HubConnection] {
    const newConnection = new HubConnectionBuilder()
        .withUrl("http://localhost:5000/searchHub")
        .withAutomaticReconnect()
        .build();
    const [connection, setConnection] = useState(newConnection);

    newConnection.start().then(() => {
        if(connection.state === HubConnectionState.Connected){
            setConnection(Object.assign({}, connection));
        }
    });

    return [connection]
}

export function useCommands(connection: HubConnection) : [PluginCommand[], (searchString: string) => void, () => void]{
    const [ searchResults, setSearchResults ] = useState<PluginCommand[]>([]);
    const updateCommandsMethods = async (searchString: string) => {
        await connection.invoke(updateCommandsMethod, searchString);
    }

    connection.on(receiveCommandsMethod, (SearchResults: PluginCommand[]) => {
        setSearchResults(SearchResults.slice(0,8));
    })

    const clearCommands = () => {
        setSearchResults([]);
    }

    return [searchResults, updateCommandsMethods, clearCommands];
}

export function useSelectedCommand(commands: PluginCommand[]) : [ number, () => void, () => void, () => void]{
    const [ activeIndex, setActiveIndex ] = useState(0);

    const moveDown = () => {
        if(activeIndex < commands.length)
            setActiveIndex(activeIndex + 1);
    }

    const moveUp = () => {
        if(activeIndex > 0)
            setActiveIndex(activeIndex - 1);
    }

    const reset = () => {
        setActiveIndex(0);
    }

    return [ activeIndex, moveUp, moveDown, reset ]
}

export function executeCommand(connection: HubConnection, command: PluginCommand) {
    connection.invoke(executeCommandMethod, command.CommandId)
        .catch(function (err : any) {
            return console.error(err.toString());
        });
}

