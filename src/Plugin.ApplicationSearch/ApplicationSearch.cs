using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;

namespace ApplicationSearch;

[Plugin("Application Search", "", "", "", "")]
public class ApplicationSearch : IIndexablePlugin
{
    private readonly IJoaLogger _joaLogger;
    private readonly ApplicationSearchSettings _settings;
    private readonly IIconHelper _iconHelper;

    public ApplicationSearch(IJoaLogger joaLogger, IIconHelper iconHelper, ApplicationSearchSettings settings)
    {
        _joaLogger = joaLogger;
        _iconHelper = iconHelper;
        _settings = settings;
    }
    
    public void ConfigurePlugin(IPluginBuilder builder)
    {
        builder.AddGlobalProvider<ApplicationProvider>();
    }

    public List<ISearchResult> CachedResults { get; set; } = new();

    public void UpdateIndexes()
    {
        _joaLogger.Info("Updating Indexes");
        
        CachedResults.Clear();
        
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
                
            CachedResults.Add(new ApplicationSearchResult
            {
                Caption = Path.GetFileNameWithoutExtension(path),
                Description = "",
                Icon = iconLocation,
                FilePath = path
            });
        }
    }
}