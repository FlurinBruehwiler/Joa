namespace JoaPluginsPackage.Plugin;

public interface IGlobalSearchPlugin : ISearchPlugin
{
    public List<SearchResult> GlobalSearchResults { get; set; }
    public void UpdateIndex();
}