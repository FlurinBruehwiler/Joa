import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {useState} from "react";

export function useJoaSettings() : [HubConnection] {
    const [connection, setConnection] = useState(
        new HubConnectionBuilder()
            .withUrl("http://localhost:5000/searchHub")
            .withAutomaticReconnect()
            .build());

    return [connection]
}
