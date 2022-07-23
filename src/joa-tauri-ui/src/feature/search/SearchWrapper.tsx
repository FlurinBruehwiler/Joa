import Search from "./Search";
import {HubConnectionState} from "@microsoft/signalr";
import {useJoaSearch} from "./services/searchService";

const SearchWrapper = () => {
    const [ connection ] = useJoaSearch();

    return (
        <div>
            { connection && connection.state === HubConnectionState.Connected && <Search connection={connection}/>}
        </div>
    );
}

export default SearchWrapper;
