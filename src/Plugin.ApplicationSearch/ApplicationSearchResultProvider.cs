using System.Text.Json;
using JoaPluginsPackage;
using JoaPluginsPackage.Enums;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Providers;

namespace ApplicationSearch;

public class ApplicationSearchResultProvider : ISearchResultProvider
{
    private readonly IJoaLogger _joaLogger;
    private readonly IIconHelper _iconHelper;
    private readonly ApplicationSearchSettings _settings;

    public ApplicationSearchResultProvider(IJoaLogger joaLogger, IIconHelper iconHelper, ApplicationSearchSettings settings)
    {
        _joaLogger = joaLogger;
        _iconHelper = iconHelper;
        _settings = settings;
    }

    public SearchResultLifetime SearchResultLifetime { get; set; }
    
    public IEnumerable<ISearchResult> GetSearchResults(string searchString)
    {
        _joaLogger.Info("Updating Indexes");
        

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
                
            yield return new ApplicationSearchResult
            {
                Caption = Path.GetFileNameWithoutExtension(path),
                Description = "",
                Icon = iconLocation,
                FilePath = path
            };
        }
    }
}