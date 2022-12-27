﻿using Joa.Settings;
using Joa.Step;
using JoaLauncher.Api;
using JoaLauncher.Api.Attributes;
using JoaLauncher.Api.Plugin;
using Microsoft.Extensions.Options;

namespace Joa.PluginCore;

public class PluginDefinition
{
    public Guid Id { get; }
    public required IPlugin Plugin { get; set; }
    public required PluginAttribute PluginInfo { get; set; }
    public required List<ProviderWrapper> GlobalProviders { get; set; }
    public SettingOption Setting { get; set; }
    public required List<ICache> Caches { get; set; }
    public required List<IAsyncCache> AsyncCaches { get; set; }
    public SettingConfiguration SettingConfiguration { get; set; }
    
    public PluginDefinition(SettingOption setting)
    {
        Id = Guid.NewGuid();
        Setting = setting;
        SettingConfiguration = new SettingConfiguration(setting);
    }
}