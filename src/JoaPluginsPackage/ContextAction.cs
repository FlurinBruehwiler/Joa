using JoaPluginsPackage.Enums;

namespace JoaPluginsPackage;

public record ContextAction
{
    public string Key { get; init; } = default!;
    public string Name { get; init; } = default!;
    public Key KeyBoardKey { get; init; } = default!;
    public ExecutionType ExecutionType { get; set; }
}

public enum ExecutionType
{
    SubResult,
    Execution
}