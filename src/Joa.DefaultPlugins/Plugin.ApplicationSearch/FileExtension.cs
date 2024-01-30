using JoaLauncher.Api.Attributes;

namespace ApplicationSearch;

public record FileExtension
{
    [SettingProperty]
    public string Extension { get; init; } = default!;
}