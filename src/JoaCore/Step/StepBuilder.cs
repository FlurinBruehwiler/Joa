using JoaPluginsPackage;
using JoaPluginsPackage.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore.Step;

public class StepBuilder : IStepBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public StepBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
            }).ToList()
        };
    }
}