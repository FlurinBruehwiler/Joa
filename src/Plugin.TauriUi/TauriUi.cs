using System.Diagnostics;
using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Plugin;

namespace TauriUi;

[Plugin("TauriUi", "Interact with Github", "1.0", "Core", "")]
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

    public void ConfigurePlugin(IPluginBuilder builder) { }
}