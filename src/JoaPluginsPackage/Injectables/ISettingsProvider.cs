namespace JoaPluginsPackage.Injectables;

public interface ISettingsProvider
{
    public T GetSetting<T>();
}