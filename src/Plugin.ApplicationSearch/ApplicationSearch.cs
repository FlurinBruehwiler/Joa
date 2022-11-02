using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Plugin;

namespace ApplicationSearch;

[Plugin("Application Search", "", "", "", "")]
public class ApplicationSearch : IPlugin
{ 
    public void ConfigurePlugin(IPluginBuilder builder)
    {
        builder.AddGlobalProvider<ApplicationProvider>()
            .AddSetting<ApplicationSearchSettings>();
    }
}