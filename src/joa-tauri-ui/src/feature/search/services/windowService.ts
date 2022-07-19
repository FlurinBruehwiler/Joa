import {appWindow, availableMonitors, LogicalPosition, Monitor} from "@tauri-apps/api/window";

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
    appWindow.setPosition(new LogicalPosition(centerOfScreenX - 300,topThirdOfScreenY - 30)).then(() => {
        appWindow.show().then(() => {
            appWindow.setFocus();
        });
    });
}

await appWindow.listen('tauri://blur', ({_}) => hideWindow());
const hideWindow = () => {
    await appWindow.hide();
}

export { showWindow, hideWindow }