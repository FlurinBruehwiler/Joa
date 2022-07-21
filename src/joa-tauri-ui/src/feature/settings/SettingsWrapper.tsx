import {useJoaSettings} from "./services/settingsService";
import Settings from "./Settings";

export default () => {
    const [ connection ] = useJoaSettings();

    return (
        <div>
            { connection && <Settings connection={connection}/>}
        </div>
    );
}
