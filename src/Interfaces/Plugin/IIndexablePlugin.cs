namespace Interfaces.Plugin;

public interface IIndexablePlugin : IPlugin
{
    public void UpdateIndex();
    public List<ISearchResult> SearchResults { get; set; }
}