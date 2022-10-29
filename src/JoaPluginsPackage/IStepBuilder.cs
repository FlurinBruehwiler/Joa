namespace JoaPluginsPackage;

public class IStepBuilder
{
    public IStepBuilder AddProvider<T>(ISearchProviderContext context) where T : ISearchResultProvider
    {
        return this;
    }
    
    public IStepBuilder AddProvider<T>() where T : ISearchResultProvider
    {
        return this;
    }
    
    public IStepBuilder AddCustomName()
    {
        return this;
    }
}