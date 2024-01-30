namespace WebSearch;

public static class DefaultSearchEngines
{
    public static SearchEngine Google { get; set; } = new()
    {
        Name = "Google",
        Prefix = "g?",
        Url = "https://google.com/search?q={{query}}",
        SuggestionUrl = "https://www.google.com/complete/search?client=opera&q={{query}}",
        IconType = IconType.Svg,
        Icon = "https://google.com/favicon.ico",
        Priority = 2,
        Fallback = false,
        EncodeSearchTerm = true
    };

    public static SearchEngine Youtube { get; set; } = new()
    {
        Name = "Youtube",
        Prefix = "y?",
        Url = "https://www.youtube.com/results?search_query={{query}}",
        SuggestionUrl = "https://www.google.com/complete/search?ds=yt&output=firefox&q={{query}}",
        IconType = IconType.Svg,
        Icon = "https://www.youtube.com/favicon.ico",
        Priority = 5,
        Fallback = false,
        EncodeSearchTerm = true
    };
}