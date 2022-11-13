using JoaPluginsPackage;
using JoaPluginsPackage.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace JoaInterface.Step;

public class StepBuilder : IStepBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISearchResult _pluginSearchResult;

    public StepBuilder(IServiceProvider serviceProvider, ISearchResult pluginSearchResult)
    {
        _serviceProvider = serviceProvider;
        _pluginSearchResult = pluginSearchResult;
    }
    
    private List<IProvider> Providers { get; set; } = new();
    
    public IStepBuilder AddProvider<T>(params object[] parameter) where T : IProvider
    {
        Providers.Add(ActivatorUtilities.CreateInstance<T>(_serviceProvider, parameter));
        return this;
    }

    public Step Build()
    {
        return new Step
        {
            Providers = Providers.Select(x => new ProviderWrapper
            {
                Provider = x
            }).ToList(),
            Name = _pluginSearchResult.Caption
        };
    }
}