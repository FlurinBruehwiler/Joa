using JoaPluginsPackage.Attributes;

namespace ApplicationSearch;

public record ApplicationFolder()
{
    [Path]
    [SettingProperty]
    public string Path { get; init; } = default!;
}