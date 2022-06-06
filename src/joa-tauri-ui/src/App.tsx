import React, {useCallback, useEffect, useState} from 'react';
import {HubConnectionBuilder,} from '@microsoft/signalr';
import {appWindow, LogicalPosition, LogicalSize, availableMonitors} from '@tauri-apps/api/window'

function App() {
    const [ joaCore, setJoaCore ] = useState<any | undefined>(undefined);
    const [ searchString, setSearchString ] = useState<string>("");
    const [ searchResults, setSearchResults ] = useState<any>([]);
    const [ activeIndex, setActiveIndex ] = useState(0);

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl("http://localhost:5000/searchHub")
            .withAutomaticReconnect()
            .build();
        setJoaCore(newConnection);
    }, []);
    useEffect(() => {
        if (joaCore) {
            joaCore.start({ withCredentials: false })
                .then(() => {
                    joaCore.on("ReceiveSearchResults", (SearchResults: any) => {
                        setSearchResults(SearchResults.slice(0,8));
                    });
                    joaCore.on("ShowWindow", (posX: number, posY: number) => {
                        availableMonitors().then(monitors => {
                            monitors.map(monitor => {
                                if(monitor.position.x < posX && monitor.position.y < posY){
                                    if(monitor.position.x + monitor.size.width > posX && monitor.position.y + monitor.size.height > posY){
                                        let centerOfScreenX = monitor.position.x + (monitor.size.width / 2);
                                        let topThirdOfScreenY = monitor.position.y + (monitor.size.height / 3);
                                        appWindow.setPosition(new LogicalPosition(centerOfScreenX - 300,topThirdOfScreenY - 30)).then(() => {
                                            appWindow.show().then(() => {
                                                appWindow.setFocus();
                                            });
                                        });
                                    }
                                }
                            });
                        });

                    });
                })
                .catch((e: any) => console.log('Connection failed: ', e));
        }
    }, [joaCore]);
    appWindow.listen('tauri://blur', ({event, payload}) => hideSearchWindow());
    const hideSearchWindow = () => {
        appWindow.hide();
        setSearchResults([]);
        setSearchString("");
    }
    const searchStringChanged = (e : any) => setSearchString(e.target.value);
    useEffect(() => {
        if(joaCore)
            joaCore
                .invoke("GetSearchResults", searchString)
                .catch(function (err : any) {});
    }, [searchString])
    useEffect(() => {
        appWindow.setSize(new LogicalSize(600, 60 + 50 * (searchResults ? searchResults?.length : 0)));
        setActiveIndex(0);
    }, [searchResults])
    const handleInputKeyPress = (e : React.KeyboardEvent) => {
        if(e.key === 'ArrowDown' && activeIndex < searchResults.length){
            setActiveIndex(activeIndex + 1);
        }
        if(e.key === 'ArrowUp' && activeIndex > 0){
            setActiveIndex(activeIndex - 1)
        }
        if(e.key === 'Enter' && searchResults.length > 0){
            joaCore.invoke("ExecuteSearchResult", searchResults[activeIndex].commandId)
                .catch(function (err : any) {
                return console.error(err.toString());
            });
        }
    }
    const handleKeyPress = useCallback((event: any) => {
        if(event.key === 'Escape')
            hideSearchWindow();
    }, []);
    useEffect(() => {
        document.addEventListener('keydown', handleKeyPress);
        return () => {
            document.removeEventListener('keydown', handleKeyPress);
        };
    }, [handleKeyPress]);

    return (
      <>
          <div className="w-full h-[60px] text-userInputText flex bg-userInputBackground items-center" data-tauri-drag-region>
              <svg className="fill-userInputText w-[28px] h-[28px] m-[16px]" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32" version="1.1" data-tauri-drag-region>
                  <g id="surface1">
                      <path
                          d="M 19 3 C 13.488281 3 9 7.488281 9 13 C 9 15.394531 9.839844 17.589844 11.25 19.3125 L 3.28125 27.28125 L 4.71875 28.71875 L 12.6875 20.75 C 14.410156 22.160156 16.605469 23 19 23 C 24.511719 23 29 18.511719 29 13 C 29 7.488281 24.511719 3 19 3 Z M 19 5 C 23.429688 5 27 8.570313 27 13 C 27 17.429688 23.429688 21 19 21 C 14.570313 21 11 17.429688 11 13 C 11 8.570313 14.570313 5 19 5 Z "></path>
                  </g>
              </svg>
              <input className="appearance-none focus:outline-none w-full h-full bg-userInputBackground text-[24px] font-[200]" type="text" data-tauri-drag-region
                     value={searchString}
                     onChange={searchStringChanged}
                     onKeyDown={handleInputKeyPress}
                     autoFocus
              />
          </div>
          { searchResults.map((pluginCommand :any, index : number) =>
            <div key={pluginCommand.commandId} className={`w-full h-[50px] text-userInputText ${index == activeIndex ? 'bg-searchResultActiveBackground' : 'bg-searchResultBackground' } items-center flex`}>
                <div className="w-[60px]"></div>
                <div>
                    <p className="text-[17px] text-searchResultNameText">{pluginCommand.command.caption}</p>
                    <p className="text-[12px] text-searchResultDescriptionText">{pluginCommand.commandId}</p>
                </div>
            </div>
          ) }
      </>
  );
}

export default App;
