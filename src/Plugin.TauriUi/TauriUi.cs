using System.Diagnostics;
using JoaPluginsPackage.Plugin;

namespace TauriUi;

public class TauriUi : IUiPlugin
{
    const string PathToExe = "./joa-tauri-ui.exe";
    
    public void Start(string port)
    {
        Process.Start(PathToExe);
    }

    public void Stop()
    {
        //ToDo 
    }
}