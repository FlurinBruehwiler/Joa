namespace Joa.Api.Injectables;

public interface ISettingsProvider
{
    public T GetSetting<T>();
}