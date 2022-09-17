import {
    appWindow,
    LogicalPosition,
    LogicalSize, PhysicalPosition, PhysicalSize,
    primaryMonitor
} from "@tauri-apps/api/window";
import {HubConnection} from "@microsoft/signalr";
import {useEffect} from "react";
import {register, unregisterAll} from "@tauri-apps/api/globalShortcut";
import exp from "constants";

const windowWidth = 600;
const windowHeight = 60;

const showWindow = async () => {
    const monitor = await primaryMonitor();
    if(!monitor)
        return;
    const centerOfScreenX = monitor.position.x + (monitor.size.width / 2);
    const topThirdOfScreenY = monitor.position.y + (monitor.size.height / 3);
    const targetX = centerOfScreenX - (windowWidth / 2);
    const targetY = topThirdOfScreenY - (windowHeight / 2);
    const post = new PhysicalPosition(Math.floor(targetX), Math.floor(targetY));

    await appWindow.setPosition(post);
    await appWindow.show();

    await appWindow.setFocus();
}

export function useActivationKey(){
    useEffect(() => {
        unregisterAll().then();
        register("Alt+P", async () => {
            await showWindow();
        }).then();
        return () => {
            unregisterAll().then();
        }
    }, [])
}

export function useWindow(connection: HubConnection, clearCommands: () => void, clearSelectedCommand: () => void, clearSearchString: () => void) : [((numOfCommands: number) => void)] {
    const handleEscape = async (event: any) => {
        if (event.key !== 'Escape')
            return;

        await hideSearchWindow();
    }

    useEffect(() => {
        document.addEventListener('keydown', handleEscape);

        let unlistenFn: () => void;

        //appWindow.listen('tauri://blur', ({event, payload}) => hideSearchWindow()).then((x: () => void) => unlistenFn = x);
        
        return () => {
            document.removeEventListener('keydown', handleEscape);
            //unlistenFn();
        };
    }, []);


    const updateSize = async (numOfCommands: number) => {
        await appWindow.setSize(new LogicalSize(windowWidth, windowHeight + 50 * numOfCommands));
    }

    const hideSearchWindow = async () => {
        console.log("hide wiow");
        await appWindow.hide()
        clearCommands();
        clearSelectedCommand();
        clearSearchString();
    }

    return [ updateSize ]
}
