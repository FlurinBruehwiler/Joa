import Search from "./Search";
import {useJoaSearch} from "./services/searchService";
import {HubConnectionState} from "@microsoft/signalr";
import {useEffect} from "react";

export default () => {
    const [ connection ] = useJoaSearch();

    useEffect(() => {
        console.log("rerendering");
    })

    return (
        <div>
            {
                connection.state === HubConnectionState.Connected && <Search connection={connection}/>
            }
        </div>
    );
}
