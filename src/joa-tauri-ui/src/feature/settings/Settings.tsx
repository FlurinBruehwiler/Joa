import React, {useEffect, useState} from 'react';
import {HubConnectionBuilder} from "@microsoft/signalr";

export default () => {
    const [ joaCore, setJoaCore ] = useState<any | undefined>(undefined);
    useEffect(() => {
        const interval = setInterval(() => {
            if (joaCore)
                return;
            tryEstablishConnection();
        }, 1000);
        return () => clearInterval(interval);
    }, []);
    const tryEstablishConnection = () => {
        console.log("trying to establish connection from settings");
        const newConnection = new HubConnectionBuilder()
            .withUrl("http://localhost:5000/settingsHub")
            .withAutomaticReconnect()
            .build();
        setJoaCore(newConnection);
    }
    useEffect(() => {
        console.log("rerendering");
        if (joaCore) {
            joaCore.start({ withCredentials: false })
                .then(() => {
                    joaCore.on("ReceiveSettings", (settings: any) => {
                        console.log(JSON.stringify(settings));
                    });
                    joaCore.invoke("GetSettings").catch(function (err : any) {});
                })
                .catch((e: any) => console.log('Connection failed: ', e));
        }
    }, [joaCore]);
    return (
      <div>Test</div>
    );
}