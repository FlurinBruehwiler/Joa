namespace JoaPluginsPackage.Plugin.Search;

public interface INestedSearchPlugin : ISearchPlugin
{
    public List<ISearchResult> GetNestedSearchResults(ISearchContext context);
}