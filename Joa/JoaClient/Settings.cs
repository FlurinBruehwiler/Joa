using Interfaces;
using Interfaces.Settings;
using Newtonsoft.Json;

namespace AppWithPlugin;

public class Settings : ISettings
{
    public List<Setting> PluginSettings { get; set; }
    
    // public Settings()
    // {
    //     var pluginLoader = new PluginLoader();
    //     var plugins = pluginLoader.GetPlugins();
    //     
    //     var settingsFilePath = Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location, @"..\..\..\..\..\settings.json"));
    //
    //     using (var reader = new StreamReader(settingsFilePath))
    //     {
    //         var json = reader.ReadToEnd();
    //         
    //         if(json == string.Empty)
    //             PluginSettings = plugins.SelectMany(x => x.GetSettings()).ToList();
    //         else
    //             PluginSettings = JsonConvert.DeserializeObject<List<Setting>>(json) ?? throw new Exception();
    //     }
    // }
}