using Joa.Settings;
using Joa.Step;
using JoaLauncher.Api;
using JoaLauncher.Api.Attributes;
using JoaLauncher.Api.Plugin;

namespace Joa.PluginCore;

public class PluginDefinition
{
    public Guid Id { get; }
    public required IPlugin Plugin { get; set; }
    // public required PluginAttribute PluginInfo { get; set; }
    public required List<ProviderWrapper> GlobalProviders { get; set; }
    public ISetting Setting { get; set; }
    public required List<ICache> Caches { get; set; }
    public required List<IAsyncCache> AsyncCaches { get; set; }
    public ClassInstance SettingConfiguration { get; set; }

    public PluginDefinition(ISetting setting)
    {
        Id = Guid.NewGuid();
        Setting = setting;
        SettingConfiguration = new ClassInstance(setting, new ClassDescription(setting.GetType()));
    }
}