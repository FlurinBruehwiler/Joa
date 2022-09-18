import React, {useEffect, useState} from 'react';
import {useWindow} from "./services/windowService";
import {executeCommand, useCommands, useSelectedCommand} from "./services/searchService";
import {FeatureProps} from "../../featureProps";
import PluginCommand from "./models/pluginCommand";

export default (props: FeatureProps) => {
    const [ searchString, setSearchString ] = useState<string>("");

    const clearSearchString = () => {
      setSearchString("");
    }

    const [ commands, updateCommands, clearCommands ] = useCommands(props.connection, searchString);
    const [ selectedCommandIndex, moveUp, moveDown, clearSelectedCommand ] = useSelectedCommand(commands);
    const [ updateSize ] = useWindow(props.connection, clearCommands, clearSelectedCommand, clearSearchString);

    const handleInputKeyPress = (e : React.KeyboardEvent) => {
        switch (e.key) {
            case 'ArrowDown':
                moveDown();
                break;
            case 'ArrowUp':
                moveUp();
                break;
            case 'Enter':
                executeCommand(props.connection, commands[selectedCommandIndex])
                break;
        }
    }

    useEffect(() => {
        updateCommands(searchString);
    }, [searchString])

    useEffect(() => {
        updateSize(commands.length);
        clearSelectedCommand();
    }, [commands])

    return (
      <>
          <div className="w-full h-[60px] text-userInputText flex bg-userInputBackground items-center overflow-hidden" data-tauri-drag-region>
              <svg className="fill-userInputText w-[28px] h-[28px] m-[16px]" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32" version="1.1" data-tauri-drag-region>
                  <g id="surface1">
                      <path
                          d="M 19 3 C 13.488281 3 9 7.488281 9 13 C 9 15.394531 9.839844 17.589844 11.25 19.3125 L 3.28125 27.28125 L 4.71875 28.71875 L 12.6875 20.75 C 14.410156 22.160156 16.605469 23 19 23 C 24.511719 23 29 18.511719 29 13 C 29 7.488281 24.511719 3 19 3 Z M 19 5 C 23.429688 5 27 8.570313 27 13 C 27 17.429688 23.429688 21 19 21 C 14.570313 21 11 17.429688 11 13 C 11 8.570313 14.570313 5 19 5 Z "></path>
                  </g>
              </svg>
              <input className="appearance-none focus:outline-none w-full h-full bg-userInputBackground text-[24px] font-[200] overflow-hidden" type="text" data-tauri-drag-region
                     value={searchString}
                     onChange={(e : any) => setSearchString(e.target.value)}
                     onKeyDown={handleInputKeyPress}
                     autoFocus
              />
          </div>
          { commands.map((pluginCommand : PluginCommand, index : number) =>
            <div key={pluginCommand.commandId} className={`w-full h-[50px] text-userInputText ${index == selectedCommandIndex ? 'bg-searchResultActiveBackground' : 'bg-searchResultBackground' } items-center flex`}>
                <div className="w-[60px] h-full flex items-center justify-center">
                    <img src={pluginCommand.searchResult.webIcon} alt=""/>
                </div>
                <div>
                    <p className="text-[17px] text-searchResultNameText">{pluginCommand.searchResult.caption}</p>
                    <p className="text-[12px] text-searchResultDescriptionText whitespace-nowrap">{pluginCommand.searchResult.description}</p>
                </div>
            </div>
          ) }
      </>
  );
}
