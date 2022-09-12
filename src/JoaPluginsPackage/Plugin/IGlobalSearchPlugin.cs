namespace JoaPluginsPackage.Plugin;

public interface IGlobalSearchPlugin : ISearchPlugin
{
    public List<ISearchResult> GlobalSearchResults { get; set; }
    public void UpdateIndex();
}