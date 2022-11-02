using JoaPluginsPackage.Providers;

namespace JoaPluginsPackage;

public class IStepBuilder
{
    public IStepBuilder AddProvider<T>(ISearchProviderContext context) where T : IProvider
    {
        return this;
    }
    
    public IStepBuilder AddProvider<T>() where T : IProvider
    {
        return this;
    }
    
    public IStepBuilder AddCustomName()
    {
        return this;
    }
}