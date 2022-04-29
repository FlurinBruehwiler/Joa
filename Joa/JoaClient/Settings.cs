using PluginBase;

namespace AppWithPlugin;

public class Settings : ISettings
{
    public string Name { get; set; } = "Test";
}