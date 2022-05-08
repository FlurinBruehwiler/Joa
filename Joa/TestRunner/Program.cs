using System.Text.Json;
using JoaCore;

var search = new Search();
var filename = Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location, @"..\..\..\..\..\settings.json"));
var options = new JsonSerializerOptions { WriteIndented = true };
var dtoSetting = new DtoSetting(search.Settings);
var jsonString = JsonSerializer.Serialize(dtoSetting, options);
File.WriteAllText(filename, jsonString);