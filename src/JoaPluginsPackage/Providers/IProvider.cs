namespace JoaPluginsPackage.Providers;

public interface IProvider
{
    public List<ISearchResult> GetSearchResults(string searchString);
}