using JoaPluginsPackage.Attributes;
using OperatingSystem = JoaPluginsPackage.Enums.OperatingSystem;

namespace ApplicationSearch;

public record ApplicationFolder
{
    [Path]
    [SettingProperty]
    public string Path { get; init; } = default!;
    
    [OperatingSystem]
    [SettingProperty]
    public OperatingSystem OperatingSystem { get; init; }
}