using Joa.Api;
using Joa.Api.Attributes;
using Joa.Api.Injectables;
using Joa.Api.Plugin;
using Joa.Api.Providers;

namespace ApplicationSearch;

[Plugin("Application Search", "", "", "", "")]
public class ApplicationSearch : ICache, IProvider, IPlugin
{
    private readonly IJoaLogger _joaLogger;
    private readonly Settings _settings;
    private readonly IIconHelper _iconHelper;
    private readonly List<ISearchResult> _searchResults = new();


    public ApplicationSearch(IJoaLogger joaLogger, IIconHelper iconHelper, Settings settings)
    {
        _joaLogger = joaLogger;
        _iconHelper = iconHelper;
        _settings = settings;
    }
    
    public void UpdateIndexes()
    {
        _joaLogger.Info("Updating Indexes");
        
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
            if (!_settings.Extensions.Any(x => path.EndsWith(x.Extension, StringComparison.OrdinalIgnoreCase))) continue;

            var iconLocation = _iconHelper.CreateIconFromFileIfNotExists<ApplicationSearch>(path);
                
            _searchResults.Add(new SearchResult
            {
                Title = Path.GetFileNameWithoutExtension(path),
                Description = "",
                Icon = iconLocation,
                FilePath = path
            });
        }
    }

    public List<ISearchResult> GetSearchResults(string searchString)
    {
        return _searchResults;
    }

    public void ConfigurePlugin(IPluginBuilder builder)
    {
        builder.AddGlobalProvider<ApplicationSearch>();
    }
}