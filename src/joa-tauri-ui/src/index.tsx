import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import Search from './feature/search/Search';
import Settings from './feature/settings/Settings';

import reportWebVitals from './reportWebVitals';
import {WebviewWindow} from "@tauri-apps/api/window";
import {
    BrowserRouter,
    Routes,
    Route,
} from "react-router-dom";

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

//const webview = new WebviewWindow('settings', {
 //   url: 'http://localhost:3000/settings'
//})

root.render(
  <React.StrictMode>
      <BrowserRouter>
          <Routes>
              <Route path="/" element={<Search/>}/>
              <Route path="/settings" element={ <Settings/> }></Route>
          </Routes>
      </BrowserRouter>
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
