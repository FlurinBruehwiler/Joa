using Github.SearchResults;
using JoaLauncher.Api;
using JoaLauncher.Api.Plugin;

namespace Github;

public class Github : IPlugin
{
    public void ConfigurePlugin(IPluginBuilder builder)
    {
        builder.AddGlobalResult(new RepositoriesSearchResult
        {
            Title = "Github Repos",
            Description = "Search for Github Repos",
            Icon = string.Empty
        });
    }
}