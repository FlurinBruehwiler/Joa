namespace JoaPluginsPackage.Plugin.Search;

public interface IStrictSearchPlugin : ISearchPlugin
{
    public bool Validator(string seachString);
    public List<SearchResult> GetStrictSearchResults(string searchString);
}