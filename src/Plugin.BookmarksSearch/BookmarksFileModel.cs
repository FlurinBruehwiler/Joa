namespace BookmarksSearch;

public class BookmarksFileModel
{
    public Roots roots { get; set; } = null!;
}

public class BookmarkBar
{
    public List<Bookmark> children { get; set; } = null!;
}

public class Bookmark
{
    public string name { get; set; } = null!;
    public string url { get; set; } = null!;
}

public class Roots
{
    public BookmarkBar bookmark_bar { get; set; } = null!;
}