using JoaLauncher.Api;
using JoaLauncher.Api.Injectables;
using JoaLauncher.Api.Plugin;
using JoaLauncher.Api.Providers;
using Microsoft.Extensions.Logging;

namespace ApplicationSearch;

public class ApplicationSearch : ICache, IProvider, IPlugin
{
    private readonly Settings _settings;
    private readonly ILogger<ApplicationSearch> _joaLogger;
    private readonly IIconHelper _iconHelper;
    private readonly List<SearchResult> _searchResults = new();
    
    public ApplicationSearch(ILogger<ApplicationSearch> joaLogger, IIconHelper iconHelper, Settings settings)
    {
        _joaLogger = joaLogger;
        _iconHelper = iconHelper;
        _settings = settings;
    }

    public void UpdateIndexes()
    {
        _joaLogger.LogInformation("Updating Indexes");

        _searchResults.Clear();

        var paths = new List<string>();

        foreach (var applicationFolder in _settings.Folders)
        {
            if (!Directory.Exists(applicationFolder.Path))
                continue;

            paths.AddRange(Directory.GetFiles(applicationFolder.Path, "*", SearchOption.AllDirectories));
        }

        foreach (var path in paths)
        {
            if (!_settings.Extensions.Any(x => path.EndsWith(x.Extension, StringComparison.OrdinalIgnoreCase)))
                continue;

            var iconLocation = _iconHelper.CreateIconFromFileIfNotExists<ApplicationSearch>(path);

            _searchResults.Add(new ApplicationSearchResult
            {
                Title = Path.GetFileNameWithoutExtension(path),
                Description = "",
                Icon = iconLocation,
                FilePath = path
            });
        }
    }

    public List<SearchResult> GetSearchResults(string searchString)
    {
        return _searchResults;
    }

    public void ConfigurePlugin(IPluginBuilder builder)
    {
        builder.AddGlobalProvider<ApplicationSearch>()
            .AddSaveAction(nameof(Settings.Folders), (Settings s) =>
            {
                UpdateIndexes();
            });
    }
}