namespace JoaPluginsPackage.Plugin.Search;

public interface INestedSearchPlugin : ISearchPlugin
{
    public List<SearchResult> GetNestedSearchResults(ISearchContext context);
}