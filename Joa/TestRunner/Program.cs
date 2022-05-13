using System.Reflection.Metadata;
using System.Text.Json;
using JoaCore;

var search = new Search();
var filename = Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location,
    @"..\..\..\..\..\settings.json"));
var options = new JsonSerializerOptions
{
    WriteIndented = true
};


var pluginLoader = new PluginLoader();
var coreSettings = new CoreSettings();
var plugins = pluginLoader.InstantiatePlugins(coreSettings);
var settings = new Settings(coreSettings,
    plugins);
var dtoSetting = new DtoSetting(settings);
var jsonString = JsonSerializer.Serialize(dtoSetting, options);
File.WriteAllText(filename,
    jsonString);
var result = JsonSerializer.Deserialize<DtoSetting>(jsonString);
Console.Write("4124");