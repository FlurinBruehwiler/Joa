﻿namespace JoaPluginsPackage;

public class NestedSearchContext
{
    public string SearchString { get; set; }
    public PathElement[] Path { get; set; }
    public SearchResult SearchResult { get; set; }
}