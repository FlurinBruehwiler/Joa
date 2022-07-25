import {HubConnection, HubConnectionBuilder, RetryContext} from "@microsoft/signalr";
import {useEffect, useState} from "react";
import {executeCommandMethod, receiveCommandsMethod, updateCommandsMethod} from "../models/joaMethods";
import PluginCommand from "../models/pluginCommand";

export function useJoaSearch() : [HubConnection | undefined] {
    const [connection, setConnection] = useState<HubConnection>();

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl("http://localhost:5000/searchHub")
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds(retryContext: RetryContext): number | null {
                    return 1000;
                }})
            .build();

        newConnection.onreconnected(() => {
            setConnection(structuredClone(connection));
        })

        newConnection.start().then(() => {
            console.log("starting connection");
            setConnection(newConnection);
        });

        return () => {
            console.log("stopping connection");
            newConnection.stop().then();
        }
    }, []);

    return [connection]
}

export function useCommands(connection: HubConnection) : [PluginCommand[], (searchString: string) => void, () => void]{
    const [ searchResults, setSearchResults ] = useState<PluginCommand[]>([]);
    const updateCommands = async (searchString: string) => {
        console.log("searching...");
        await connection.invoke(updateCommandsMethod, searchString);
    }


    useEffect(() => {
        connection.on(receiveCommandsMethod, (SearchResults: PluginCommand[]) => {
            setSearchResults(SearchResults.slice(0,8));
        });

        return () => connection.off(receiveCommandsMethod);
    }, [])


    const clearCommands = () => {
        setSearchResults([]);
    }

    return [searchResults, updateCommands, clearCommands];
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
    connection.invoke(executeCommandMethod, command.commandId)
        .catch(function (err : any) {
            return console.error(err.toString());
        });
}
