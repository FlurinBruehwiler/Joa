namespace Joa.PluginCore;

public class PluginTypes
{
    public PluginManifest PluginManifest { get; set; }
    public Type? Plugin { get; set; }
    public Type? Setting { get; set; }
    public List<Type> Caches { get; set; } = new();
    public List<Type> AsyncCaches { get; set; } = new();
}
