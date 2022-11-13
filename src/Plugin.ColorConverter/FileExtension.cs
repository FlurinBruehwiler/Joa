using Joa.Api.Attributes;

namespace ColorConverter;

public record FileExtension
{
    [SettingProperty]
    public string Extensions { get; init; } = null!;
}