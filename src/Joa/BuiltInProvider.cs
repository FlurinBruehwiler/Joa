using JoaLauncher.Api;
using JoaLauncher.Api.Providers;

namespace Joa;

public class BuiltInProvider : IProvider
{
    private readonly List<SearchResult> _searchResults;

    public BuiltInProvider(IServiceProvider serviceProvider, JoaManager joaManager)
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
                ExecutionAction = context => { Console.WriteLine(context); }
            }
        };
    }

    public List<SearchResult> GetSearchResults(string searchString)
    {
        return _searchResults;
    }
}