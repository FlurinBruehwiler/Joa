namespace Joa.PluginCore;

public class PluginTypes
{
    public Type? Plugin { get; set; }
    public Type? Setting { get; set; }
    public List<Type> Caches { get; set; } = new();
}