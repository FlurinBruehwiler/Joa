using JoaPluginsPackage.Attributes;

namespace ColorConverter;

public record FileExtension
{
    [SettingProperty]
    public string Extensions { get; init; } = null!;
}