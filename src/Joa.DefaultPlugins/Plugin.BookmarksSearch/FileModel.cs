namespace BookmarksSearch;

public record FileModel
{
    public Roots roots { get; set; } = null!;
}

public record BookmarkBar
{
    public List<Bookmark> children { get; set; } = null!;
}

public record Bookmark
{
    public string name { get; set; } = null!;
    public string url { get; set; } = null!;
}

public record Roots
{
    public BookmarkBar bookmark_bar { get; set; } = null!;
}