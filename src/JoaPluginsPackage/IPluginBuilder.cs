using JoaPluginsPackage.Providers;

namespace JoaPluginsPackage;

public interface IPluginBuilder
{
    public IPluginBuilder AddGlobalProvider<T>() where T : IProvider;
    public IPluginBuilder AddGlobalProvider<T>(Delegate condition) where T : IProvider;
    public IPluginBuilder AddGlobalResult(ISearchResult searchResult);
    public IPluginBuilder AddSetting<T>() where T : ISetting;
}