using JoaCore.Step;
using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Plugin;

namespace JoaCore.PluginCore;

public class PluginDefinition
{
    public Guid Id { get; }
    public required IPlugin Plugin { get; set; }
    public required PluginAttribute PluginInfo { get; set; }
    public required List<ProviderWrapper> GlobalProviders { get; set; }
    public required ISetting Setting { get; set; }
    public required List<ICache> Caches { get; set; }
    
    public PluginDefinition()
    {
        Id = Guid.NewGuid();
    }
}