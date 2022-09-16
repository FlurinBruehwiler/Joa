using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;
using OperatingSystem = JoaPluginsPackage.Enums.OperatingSystem;

namespace ApplicationSearch;

[Plugin("Application Search", "", "", "", "")]
public class ApplicationSearch : IGlobalSearchPlugin
{
    private readonly IJoaLogger _joaLogger;
    public List<ISearchResult> GlobalSearchResults { get; set; } = new();
    
    public ApplicationSearch(IJoaLogger joaLogger)
    {
        _joaLogger = joaLogger;
    }
    
    public void UpdateIndex()
    {
        _joaLogger.Info("Updating Indexes");
        
        GlobalSearchResults.Clear();

        var paths = new List<string>();

        foreach (var applicationFolder in Folders)
        {
            if (!Directory.Exists(applicationFolder.Path))
                continue;
                
            paths.AddRange(Directory.GetFiles(applicationFolder.Path, "*", SearchOption.AllDirectories));
        }
        
        var iconsLocation = Path.Combine(Path.GetDirectoryName(typeof(ApplicationSearch).Assembly.Location), "Icons");
        if (!Directory.Exists(iconsLocation))
            Directory.CreateDirectory(iconsLocation);
        
        foreach (var path in paths)
        {
            _joaLogger.Info(path);

            if (!Extensions.Any(x => path.EndsWith(x.Extension, StringComparison.OrdinalIgnoreCase))) continue;
            
            var iconLocation = Path.Combine(iconsLocation, Path.ChangeExtension(Path.GetFileName(path), ".png"));

            CreateIconIfNotExists(iconLocation, path);
                
            GlobalSearchResults.Add(new ApplicationSearchResult
            {
                Caption = Path.GetFileNameWithoutExtension(path),
                Description = "",
                Icon = iconLocation,
                FilePath = path
            });
        }
        
        _joaLogger.Log(JsonSerializer.Serialize(GlobalSearchResults), IJoaLogger.LogLevel.Info);
    }

    private void CreateIconIfNotExists(string iconLocation, string path)
    {
        if (File.Exists(iconLocation)) 
            return;
        
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
            return;
        
        var icon = Icon.ExtractAssociatedIcon(path);

        if (icon is null) 
            return;

        var bitmapIcon = icon.ToBitmap();
        
        bitmapIcon.Save(iconLocation, ImageFormat.Png); 
    }

    [SettingProperty(Name = "Web Search Engines")]
    public List<ApplicationFolder> Folders { get; set; } = new()
    {
        new ApplicationFolder
        {
            Path = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData), 
                @"Microsoft\Windows\Start Menu\Programs"), 
            OperatingSystem = OperatingSystem.Windows
        },
        new ApplicationFolder
        {
            Path = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), 
                @"Microsoft\Windows\Start Menu"), 
            OperatingSystem = OperatingSystem.Windows
        },
        new ApplicationFolder
        {
            Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
            OperatingSystem = OperatingSystem.Windows
        },
    };

    [SettingProperty]
    public List<FileExtension> Extensions { get; set; } = new()
    {
        new FileExtension {Extension = ".lnk"},
        new FileExtension {Extension = ".appref-ms"},
        new FileExtension {Extension = ".url"},
        new FileExtension {Extension = ".exe"},
    };

    [SettingProperty] 
    public bool ShowFullFilePath { get; set; } = false;

    [SettingProperty] 
    public bool UseNativeIcons { get; set; } = true;
}