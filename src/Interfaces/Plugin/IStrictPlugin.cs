namespace Interfaces.Plugin;

public interface IStrictPlugin : IPlugin
{
    public bool Validator(string seachString);
    public List<ICommand> GetResults(string searchString);
}