using JoaPluginsPackage.Providers;

namespace JoaPluginsPackage;

public interface IStepBuilder
{
    public IStepBuilder AddProvider<T>(params object[] parameter) where T : IProvider;
}