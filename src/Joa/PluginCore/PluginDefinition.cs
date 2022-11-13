using JoaInterface.Step;
using Joa.Api;
using Joa.Api.Attributes;
using Joa.Api.Plugin;

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