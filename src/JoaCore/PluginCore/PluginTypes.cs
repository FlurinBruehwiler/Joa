namespace JoaCore.PluginCore;

public class PluginTypes
{
    public Type? Plugin { get; set; }
    public List<Type> Settings { get; set; } = new();
    public List<Type> Caches { get; set; } = new();
}