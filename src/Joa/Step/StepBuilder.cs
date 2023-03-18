using JoaLauncher.Api;
using JoaLauncher.Api.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Joa.Step;

public class StepBuilder : IStepBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SearchResult _pluginSearchResult;

    public StepBuilder(IServiceProvider serviceProvider, SearchResult pluginSearchResult)
    {
        _serviceProvider = serviceProvider;
        _pluginSearchResult = pluginSearchResult;
    }

    private List<IProvider> Providers { get; set; } = new();
    private StepOptions StepOptions { get; set; } = new();

    public Step Build()
    {
        return new Step
        {
            Providers = Providers.Select(x => new ProviderWrapper
            {
                Provider = x
            }).ToList(),
            Name = _pluginSearchResult.Title,
            Options = StepOptions
        };
    }

    public IStepBuilder AddProvider<T>() where T : IProvider
    {
        Providers.Add(ActivatorUtilities.CreateInstance<T>(_serviceProvider));
        return this;
    }

    public IStepBuilder AddProvider<TProvider, TContext>(TContext providerContext) where TProvider : IProvider<TContext>
    {
        if (providerContext is null)
            throw new ArgumentNullException();

        Providers.Add(ActivatorUtilities.CreateInstance<TProvider>(_serviceProvider, providerContext));
        return this;
    }

    public IStepBuilder WithOptions(StepOptions stepOptions)
    {
        StepOptions = stepOptions;
        return this;
    }
}