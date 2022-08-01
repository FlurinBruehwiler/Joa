namespace JoaPluginsPackage.Plugin.Search;

public interface ISearchPlugin : IPlugin
{
    public void Execute(SearchResult searchResult, ContextAction contextAction);
}