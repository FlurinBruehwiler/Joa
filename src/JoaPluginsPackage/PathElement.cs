namespace JoaPluginsPackage;

public record PathElement
{
    public string DisplayName { get; init; } = default!;
    public string Key { get; init; } = default!;
}