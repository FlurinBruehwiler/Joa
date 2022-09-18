import {HubConnection} from "@microsoft/signalr";
import {useEffect, useState} from "react";
import PluginCommand from "../models/pluginCommand";
import {executeCommandMethod, receiveSearchResultsMethod, updateCommandsMethod} from "../models/JoaMethods";
import { convertFileSrc } from '@tauri-apps/api/tauri';

let scores: { [key: string]: number } = {};

export function useCommands(connection: HubConnection, currentSearchString: string) : [PluginCommand[], (searchString: string) => void, () => void]{
    const [ searchResults, setSearchResults ] = useState<PluginCommand[]>([]);

    useEffect(() => {
        connection.on(receiveSearchResultsMethod, (searchString: string, commands: PluginCommand[]) => {
            console.log(Date.now() - scores[searchString]);
            const firstNCommands = commands.slice(0,8);
            firstNCommands.forEach((x) => {
                if(x.searchResult.icon === "" || x.searchResult.webIcon !== undefined)
                    return;
                x.searchResult.webIcon = convertFileSrc(x.searchResult.icon);
            })
            setSearchResults(firstNCommands);
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
