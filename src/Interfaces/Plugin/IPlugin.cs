namespace Interfaces.Plugin;

public interface IPlugin
{
    public List<ISearchResult> GetResults(string searchString);
    public void Execute(ISearchResult searchResult);
}