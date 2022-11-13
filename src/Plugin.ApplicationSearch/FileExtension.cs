using Joa.Api.Attributes;
using OperatingSystem = Joa.Api.Enums.OperatingSystem;

namespace ApplicationSearch;

public record FileExtension
{
    [SettingProperty]
    public string Extension { get; init; } = default!;

    [SettingProperty]
    [OperatingSystem]
    public OperatingSystem OperatingSystem { get; set; }
}