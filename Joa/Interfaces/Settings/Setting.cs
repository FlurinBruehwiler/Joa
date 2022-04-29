namespace Interfaces.Settings;

public abstract class Setting
{
    public string PluginName { get; set; }
    public abstract string Name { get; set; }
}