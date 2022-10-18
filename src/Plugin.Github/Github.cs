using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Plugin;

namespace Github;

[Plugin("Github", "Interact with Github", "1.0", "Core", "")]
public class Github : IPlugin
{
    public void ConfigurePlugin(IPluginBuilder builder)
    {
        builder.AddGlobalProvider<RepositoriesProvider>();
    }
}