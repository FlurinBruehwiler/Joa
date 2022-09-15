import Search from "./Search";
import {HubConnectionBuilder, HubConnectionState, RetryContext} from "@microsoft/signalr";
import {useActivationKey} from "./services/windowService";
import {useEffect, useState} from "react";

const connection = new HubConnectionBuilder()
    .withUrl("http://localhost:5000/searchHub")
    .withAutomaticReconnect({
        nextRetryDelayInMilliseconds(retryContext: RetryContext): number | null {
            console.log("retrying...");
            return 1000;
        }})
    .build();

const SearchWrapper = () => {
    useActivationKey();

    const [connectionState, setConnectionState ] = useState(false);

    useEffect(() => {
        console.log("starting connection")
        connection.start().then(() => {
            console.log("finished starting connection")
            setConnectionState(true);

            connection.onreconnected(() => {
                setConnectionState(true);
            });

            connection.onclose(() => {
                setConnectionState(false);
            });
        }).catch();

        return () => {
            connection.stop().then(() => {
                setConnectionState(false);
            });
        }
    }, []);

    useEffect(() => {
        console.log("rerendefring");
    })

    return (
        <div>
            { connectionState && <Search connection={connection}/>}
        </div>
    );
}

export default SearchWrapper;
