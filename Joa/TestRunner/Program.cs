using System.Reflection.Metadata;
using System.Text.Json;
using JoaCore;

var search = new Search();
var filename = Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location, @"..\..\..\..\..\settings.json"));
var options = new JsonSerializerOptions { WriteIndented = true };

var settings = new Test
{
    Property = new Dictionary<string, object>
    {
        { "test1", 1 },
        { "test2", "2" },
        { "test3", true },
        {
            "test4", new Dictionary<string, Dictionary<string, object>>()
        }
    }
};


var jsonString = JsonSerializer.Serialize(settings);
File.WriteAllText(filename, jsonString);
var result = JsonSerializer.Deserialize<Test>(jsonString);
Console.Write("4124");

internal class Test
{
    private Dictionary<string, object> _property;

    public Dictionary<string, object> Property
    {
        get => _property;
        set
        {
            _property = value;
            Dictionary<string, object> newDictionary = new();
            
            foreach (var (key, settingValue) in value)
            {
                if (settingValue is not JsonElement jsonElement)
                {
                    newDictionary.Add(key, settingValue);
                    continue;
                }
                
                var type = jsonElement.ValueKind switch
                {
                    JsonValueKind.Number => typeof(int),
                    JsonValueKind.String => typeof(string),
                    JsonValueKind.True => typeof(bool),
                    JsonValueKind.Array => typeof(List<>),
                    _ => throw new JsonException()
                };
            
                newDictionary.Add(key, jsonElement.Deserialize(type) ?? throw new JsonException());
            }
            
            _property = newDictionary;
        }
    }
}