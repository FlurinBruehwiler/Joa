namespace JoaInterface.SearchEngine;

public class MatchOption
{
    /// <summary>
    /// Gets or sets prefix of match char, use for highlight
    /// </summary>
    [Obsolete("this is never used")]
    public string Prefix { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets suffix of match char, use for highlight
    /// </summary>
    [Obsolete("this is never used")]
    public string Suffix { get; set; } = string.Empty;

    public bool IgnoreCase { get; set; } = true;
}