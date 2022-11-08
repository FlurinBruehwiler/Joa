using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Plugin;

namespace WebSearch;

[Plugin("Web Search", "Lets you search on the web!", "", "", "")]
public class WebSearch : IPlugin
{
    public void ConfigurePlugin(IPluginBuilder builder)
    {
        builder.AddGlobalProvider<WebProvider>(Condition);
    }

    private bool Condition(string searchString, WebSearchSettings settings)
    {
        return settings.SearchEngines.Any(x => searchString.StartsWith(x.Prefix));
    }
}
