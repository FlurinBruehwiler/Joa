namespace JoaPluginsPackage;

public record ContextAction
{
    public string Key { get; init; } = default!;
    public string Name { get; init; } = default!;
    public Key KeyBoardKey { get; init; } = default!;
    public string? Link { get; init; }
}