using JoaLauncher.Api;
using JoaLauncher.Api.Attributes;
using JoaLauncher.Api.Plugin;
using JoaInterface.Step;

namespace JoaInterface.PluginCore;

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