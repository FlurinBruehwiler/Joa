namespace JoaPluginsPackage.Injectables;

public interface IIconHelper
{
    public string GetIconsDirectory(Type pluginType);

    public void CreateIconFromFileIfNotExists(string iconLocation, string fileLocation);
}