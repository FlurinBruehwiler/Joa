using JoaPluginsPackage.Plugin;

namespace JoaPluginsPackage;

public interface IIndexablePlugin : IPlugin
{
    public void UpdateIndexes();
}