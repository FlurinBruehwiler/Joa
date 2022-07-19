import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import PluginCommand from "../models/PluginCommand";
import {showWindow} from "./windowService";

class SearchService {
    private connection: HubConnection | undefined;

    ReceiveSearchResults = "ReceiveSearchResults";
    ShowWindow = "ShowWindow";

    public receiveSearchResults: (searchResults: PluginCommand[]) => void

    connect = async () => {
        this.connection = new HubConnectionBuilder()
            .withUrl("http://localhost:5000/searchHub")
            .withAutomaticReconnect()
            .build();

        await this.connection.start({ withCredentials: false })
        this.connection.on(this.ReceiveSearchResults, (SearchResults: PluginCommand[]) => {
            this.receiveSearchResults(SearchResults);
        });
        this.connection.on(this.ShowWindow, (posX: number, posY: number) => {
            showWindow(posX, posY);
        });
    }
}

export { SearchService };