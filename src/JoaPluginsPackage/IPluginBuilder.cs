namespace JoaPluginsPackage;

public interface IPluginBuilder
{
    public IPluginBuilder AddGlobalProvider<T>() where T : ISearchResultProvider;
    public IPluginBuilder AddGlobalProvider<T>(Delegate condition) where T : ISearchResultProvider;
    public IPluginBuilder AddProvider<T>() where T : ISearchResultProvider;
    public IPluginBuilder AddGlobalResult<T>() where T : ISearchResult;
    public IPluginBuilder AddSetting<T>() where T : ISetting;
}