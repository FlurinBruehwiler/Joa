namespace JoaPluginsPackage.Plugin;

public interface INestedSearchPlugin : ISearchPlugin
{
    public List<SearchResult> GetNestedSearchResults(ISearchContext context);
}