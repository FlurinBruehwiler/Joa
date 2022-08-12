namespace JoaPluginsPackage.Plugin;

public interface INestedSearchPlugin : IGlobalSearchPlugin
{
    public List<SearchResult> GetNestedSearchResults(NestedSearchContext context);
}