using JoaPluginsPackage.Plugin;

namespace ApplicationSearch;

public class ApplicationSearchResult : SearchResult
{
    public ApplicationSearchResult(string caption, string description, string icon, string filePath) 
        : base(caption, description, icon)
    {
        FilePath = filePath;
    }
    public string FilePath { get; set; }
}