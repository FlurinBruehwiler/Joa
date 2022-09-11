namespace JoaPluginsPackage;

public record SearchResult
{
    public string Caption { get; init; } = default!;
    public string Description { get; init; } = default!;
    public string Icon { get; init; } = default!;
    public List<ContextAction>? Actions { get; init; } = new()
    {
        new ContextAction
        {
            KeyBoardKey = Key.Enter,
            Key = "enter",
            Name = "Open Application"
        }
    };
}