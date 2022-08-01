namespace JoaPluginsPackage.Plugin.Search;

public interface IGlobalSearchPlugin : ISearchPlugin
{
    public List<SearchResult> GlobalSearchResults { get; set; }
    public void UpdateIndex();
}