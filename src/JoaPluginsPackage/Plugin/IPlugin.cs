namespace JoaPluginsPackage.Plugin;

public interface IPlugin
{
    public void Execute(ICommand command);
}