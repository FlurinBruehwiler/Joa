namespace JoaPluginsPackage.Plugin.Search;

public interface ISearchPlugin : IPlugin
{
    public void Execute(ISearchResult searchResult, IAction action);
}