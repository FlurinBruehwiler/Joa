namespace Interfaces.Plugin;

public interface IIndexablePlugin : IPlugin
{
    public void UpdateIndex();
    public List<ICommand> SearchResults { get; set; }
}