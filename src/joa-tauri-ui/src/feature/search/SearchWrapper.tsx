import Search from "./Search";
import {HubConnectionState} from "@microsoft/signalr";
import {useJoaSearch} from "./services/searchService";
import {useActivationKey} from "./services/windowService";
import {useEffect} from "react";

const SearchWrapper = () => {
    const [ connection ] = useJoaSearch();
    useActivationKey();

    useEffect(() => {
        console.log(connection?.state);
    }, [connection]);

    return (
        <div>
            { connection && connection.state === HubConnectionState.Connected && <Search connection={connection}/>}
        </div>
    );
}

export default SearchWrapper;
