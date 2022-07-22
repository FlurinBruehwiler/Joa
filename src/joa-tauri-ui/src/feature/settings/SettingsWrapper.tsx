import {useEffect} from "react";
import {appWindow, LogicalSize} from "@tauri-apps/api/window";

export default () => {

    useEffect(() => {
        appWindow.show();
        appWindow.setSize(new LogicalSize(600, 600));
    }, [])

    return (
        <div>
        </div>
    );
}
