namespace BookmarksSearch;

public class BookmarksFileModel
{
    public string checksum { get; set; }
    public Roots roots { get; set; }
    public string sync_metadata { get; set; }
    public int version { get; set; } 
}

public class BookmarkBar
{
    public List<Bookmark> children { get; set; }
    public string date_added { get; set; }
    public string date_modified { get; set; }
    public string guid { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
}

public class Bookmark
{
    public string date_added { get; set; }
    public string guid { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public string url { get; set; }
}

public class Other
{
    public List<object> children { get; set; }
    public string date_added { get; set; }
    public string date_modified { get; set; }
    public string guid { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
}

public class Roots
{
    public BookmarkBar bookmark_bar { get; set; }
    public Other other { get; set; }
    public Synced synced { get; set; }
}

public class Synced
{
    public List<object> children { get; set; }
    public string date_added { get; set; }
    public string date_modified { get; set; }
    public string guid { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
}

