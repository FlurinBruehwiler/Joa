using JoaPluginsPackage.Providers;

namespace JoaPluginsPackage;

public class IStepBuilder
{
    public IStepBuilder AddProvider<T>(ISearchProviderContext context) where T : IResultProvider
    {
        return this;
    }
    
    public IStepBuilder AddProvider<T>() where T : IResultProvider
    {
        return this;
    }
    
    public IStepBuilder AddCustomName()
    {
        return this;
    }
}