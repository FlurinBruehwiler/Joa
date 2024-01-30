using JoaLauncher.Api.Attributes;

namespace ApplicationSearch;

public record Folder
{
    [Path]
    [SettingProperty]
    public string Path { get; init; } = default!;
}