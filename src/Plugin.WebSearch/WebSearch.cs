using Joa.Api;
using Joa.Api.Attributes;
using Joa.Api.Plugin;

namespace WebSearch;

[Plugin("Web Search", "Lets you search on the web!", "", "", "")]
public class WebSearch : IPlugin
{
    private readonly WebSearchSettings _settings;

    public WebSearch(WebSearchSettings settings)
    {
        _settings = settings;
    }
    
    public void ConfigurePlugin(IPluginBuilder builder)
    {
        builder.AddGlobalProvider<WebProvider>(Condition);
    }

    private bool Condition(string searchString)
    {
        return _settings.SearchEngines.Any(x => searchString.StartsWith(x.Prefix));
    }
}
