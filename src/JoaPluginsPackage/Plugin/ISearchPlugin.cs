namespace JoaPluginsPackage.Plugin;

public interface ISearchPlugin : IPlugin
{
    public void Execute(SearchResult searchResult, ContextAction contextAction);
}