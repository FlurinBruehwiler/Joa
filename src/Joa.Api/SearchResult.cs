namespace JoaLauncher.Api;

/// <summary>
/// Represents a search result which will be displayed to the user
/// </summary>
public abstract class SearchResult
{
    /// <summary>
    /// The title is a big text displayed on the search result
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// The description is a smaller text below the title
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// The Icon displayed next to the title and description
    /// Should contain the path to the icon. URLs are not supported
    /// </summary>
    public required string Icon { get; init; }

    /// <summary>
    /// A list of <see cref="ContextAction"/> which represent the
    /// possible actions the user could execute on this search result
    /// </summary>
    public List<ContextAction>? Actions { get; set; }

    /// <summary>
    /// The method which gets called if the user executes the search result
    /// </summary>
    /// <param name="executionContext"></param>
    public abstract void Execute(IExecutionContext executionContext);
}