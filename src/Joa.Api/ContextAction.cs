using Joa.Api.Enums;

namespace Joa.Api;

/// <summary>
/// A ContextAction represents a variant with which the
/// user can execute a search result
/// </summary>
public record ContextAction
{
    /// <summary>
    /// The key which is used to identify the ContextAction
    /// </summary>
    public string Key { get; init; } = default!;
    
    /// <summary>
    /// The name which will be displayed to the user
    /// </summary>
    public string Name { get; init; } = default!;
    
    /// <summary>
    /// The keyboard key that the user can use to execute
    /// the search result with this action
    /// </summary>
    public Key KeyBoardKey { get; init; } = default!;
}