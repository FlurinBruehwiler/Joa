using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Plugin;

namespace BookmarksSearch;

[Plugin("Bookmark Search", "", "", "", "")]
public class BookmarksSearch : IPlugin
{

    
    public void ConfigurePlugin(IPluginBuilder builder)
    {
        builder.AddGlobalProvider<Provider>()
            .AddSetting<Setting>()
            .AddCache<Cache>();
    }

    
}