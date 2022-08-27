import {HubConnection, HubConnectionBuilder, RetryContext} from "@microsoft/signalr";
import {useEffect, useState} from "react";
import PluginCommand from "../models/pluginCommand";
import {executeCommandMethod, receiveSearchResultsMethod, updateCommandsMethod} from "../models/JoaMethods";

export function useJoaSearch() : [HubConnection | undefined] {
    const [connection, setConnection] = useState<HubConnection|undefined>();

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl("http://localhost:5000/searchHub")
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds(retryContext: RetryContext): number | null {
                    console.log("retrying...");
                    return 1000;
                }})
            .build();

        newConnection.onreconnected(() => {
            console.log(connection === newConnection);
            console.log(newConnection.state);
            console.log(connection?.state);
            setConnection(newConnection);
        });

        newConnection.onclose(() => {
            setConnection(undefined);
        });

        newConnection.start().then(() => {
            setConnection(newConnection);
        });

        return () => {
            newConnection.stop().then();
        }
    }, []);

    return [connection]
}

let scores: { [key: string]: number } = {};

export function useCommands(connection: HubConnection, currentSearchString: string) : [PluginCommand[], (searchString: string) => void, () => void]{
    const [ searchResults, setSearchResults ] = useState<PluginCommand[]>([]);

    useEffect(() => {
        connection.on(receiveSearchResultsMethod, (searchString: string, commands: PluginCommand[]) => {
            console.log(Date.now() - scores[searchString]);
            if(currentSearchString === searchString){}
                setSearchResults(commands.slice(0,8));
        });
    }, []);

    const sendNewSearchString = async (searchString: string) => {
        scores[searchString] = Date.now();
        await connection.send(updateCommandsMethod, searchString);
    }

    const clearCommands = () => {
        setSearchResults([]);
    }

    return [searchResults, sendNewSearchString, clearCommands];
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
    console.log("execrutingoffnd");
    connection.invoke(executeCommandMethod, command.commandId, "enter")
        .catch(function (err : any) {
            return console.error(err.toString());
        });
}
