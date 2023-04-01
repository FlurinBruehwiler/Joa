using Joa.PluginCore;
using JoaLauncher.Api;
using JoaLauncher.Api.Providers;

namespace Joa;

public class BuiltInProvider : IProvider
{
    private readonly List<SearchResult> _searchResults;

    public BuiltInProvider(IServiceProvider serviceProvider, JoaManager joaManager, PluginManager pluginManager)
    {
        _searchResults = new List<SearchResult>
        {
            new BuiltInSearchResult
            {
                Title = "Settings",
                Description = "Change Settings",
                Icon = string.Empty,
                ExecutionAction = _ =>
                {
                    joaManager.ExecuteOnUiThread(() => Program.CreateSettingsWindow(serviceProvider));
                }
            },
            new BuiltInSearchResult
            {
                Title = "Refresh indexes",
                Description = "Refresh all indexes of all plugins",
                Icon = string.Empty,
                ExecutionAction = context => Task.Run(pluginManager.UpdateIndexesAsync)
            }
        };
    }

    public List<SearchResult> GetSearchResults(string searchString)
    {
        return _searchResults;
    }
}