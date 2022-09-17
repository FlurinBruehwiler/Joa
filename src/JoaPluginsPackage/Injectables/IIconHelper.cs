namespace JoaPluginsPackage.Injectables;

public interface IIconHelper
{
    public string GetIconsDirectory(Type pluginType);
    
    public string? GetIconsDirectoryIfExists(Type pluginType);
}