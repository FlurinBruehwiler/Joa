import Search from "./Search";
import {useJoaSearch} from "./services/searchService";

export default () => {
    const [ connection ] = useJoaSearch();

    return (
        <div>
            { connection && <Search connection={connection}/>}
        </div>
    );
}
