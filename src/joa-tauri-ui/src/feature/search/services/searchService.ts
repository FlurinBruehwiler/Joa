import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import PluginCommand from "../models/PluginCommand";
import {showWindow} from "./windowService";

class SearchService {

    ReceiveSearchResultsName = "ReceiveSearchResults";
    ShowWindowName = "ShowWindow";

    constructor(private connection: HubConnection) {
        this.connection.on(this.ReceiveSearchResultsName, this.receiveSearchResults);
        this.connection.on(this.ShowWindowName, this.showWindow);
    }

    receiveSearchResults = (SearchResults: PluginCommand[]) => {

    }

    showWindow = (posX: number, posY: number) => {
        showWindow(posX, posY);
    }

    getSearchResults = () => {

    }
}

export { SearchService };