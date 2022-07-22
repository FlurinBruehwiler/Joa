import {appWindow, availableMonitors, LogicalPosition, LogicalSize, Monitor} from "@tauri-apps/api/window";
import {HubConnection} from "@microsoft/signalr";
import {showWindowMethod} from "../models/JoaMethods";

const getMonitorFromMousePos = async (posX: number, posY: number) : Promise<Monitor> => {
    const monitors = await availableMonitors();
    monitors.forEach(monitor => {
        if (monitor.position.x < posX && monitor.position.y < posY){
            if (monitor.position.x + monitor.size.width > posX && monitor.position.y + monitor.size.height > posY){
                return monitor;
            }
        }
    })
    return monitors[0];
}

const showWindow = async (posX: number, posY: number) => {
    const monitor = await getMonitorFromMousePos(posX, posY);
    let centerOfScreenX = monitor.position.x + (monitor.size.width / 2);
    let topThirdOfScreenY = monitor.position.y + (monitor.size.height / 3);
    await appWindow.setPosition(new LogicalPosition(centerOfScreenX - 300,topThirdOfScreenY - 30))
    await appWindow.show()
    await appWindow.setFocus();
}

export function useWindow(connection: HubConnection, clearCommands: () => void, clearSelectedCommand: () => void ) : [(numOfCommands: number) => void] {
    console.log("useWindow");

    connection.on(showWindowMethod, async(posX: number, posY: number) => {
        await showWindow(posX, posY);
    });

    const updateSize = async (numOfCommands: number) => {
        await appWindow.setSize(new LogicalSize(600, 60 + 50 * numOfCommands));
    }

    const hideSearchWindow = async () => {
        await appWindow.hide()
    }

    document.addEventListener('keydown', async (event: any) => {
        if (event.key !== 'Escape')
            return;

        await hideSearchWindow();
        clearCommands();
        clearSelectedCommand();
    })

    appWindow.listen('tauri://blur', ({event, payload}) => hideSearchWindow());

    return [ updateSize ]
}
