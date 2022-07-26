namespace JoaPluginsPackage.Plugin.Search;

public interface IGlobalSearchPlugin : ISearchPlugin
{
    public List<ISearchResult> GlobalSearchResults { get; set; }
}