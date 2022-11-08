using JoaPluginsPackage;
using JoaPluginsPackage.Injectables;

namespace ApplicationSearch;

public class Cache : ICache
{
    private readonly IJoaLogger _joaLogger;
    private readonly Settings _settings;
    private readonly IIconHelper _iconHelper;

    public Cache(IJoaLogger joaLogger, IIconHelper iconHelper, Settings settings)
    {
        _joaLogger = joaLogger;
        _iconHelper = iconHelper;
        _settings = settings;
    }

    public List<ISearchResult> SearchResults { get; set; }
    
    public void UpdateIndexes()
    {
        _joaLogger.Info("Updating Indexes");
        
        SearchResults.Clear();
        
        var paths = new List<string>();

        foreach (var applicationFolder in _settings.Folders)
        {
            if (!Directory.Exists(applicationFolder.Path))
                continue;
                
            paths.AddRange(Directory.GetFiles(applicationFolder.Path, "*", SearchOption.AllDirectories));
        }

        foreach (var path in paths)
        {
            _joaLogger.Info(path);

            if (!_settings.Extensions.Any(x => path.EndsWith(x.Extension, StringComparison.OrdinalIgnoreCase))) continue;

            var iconLocation = _iconHelper.CreateIconFromFileIfNotExists<ApplicationSearch>(path);
                
            SearchResults.Add(new SearchResult
            {
                Caption = Path.GetFileNameWithoutExtension(path),
                Description = "",
                Icon = iconLocation,
                FilePath = path
            });
        }
    }
}