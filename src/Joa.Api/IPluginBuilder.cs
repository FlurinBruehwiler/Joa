using Joa.Api.Providers;

namespace Joa.Api;

public interface IPluginBuilder
{
    public IPluginBuilder AddGlobalProvider<T>() where T : IProvider;
    public IPluginBuilder AddGlobalProvider<T>(Func<string, bool> condition) where T : IProvider;
    public IPluginBuilder AddGlobalResult(ISearchResult searchResult);
}