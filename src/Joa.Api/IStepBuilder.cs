using JoaLauncher.Api.Providers;

namespace JoaLauncher.Api;

/// <summary>
/// Is used to Build a new Step. A Step Containt Providers
/// </summary>
public interface IStepBuilder
{
    /// <summary>
    /// Can be used to add a Provider without a ProviderContext to the Step.
    /// </summary>
    /// <typeparam name="T">The Type of Provider to be added to the StepBuilder</typeparam>
    /// <returns>The StepBuilder</returns>
    public IStepBuilder AddProvider<T>() where T : IProvider;

    /// <summary>
    /// Can be used to add a Provider with a ProviderContext to the Step.
    /// </summary>
    /// <param name="providerContext">The ProviderContext used to instantiate the Provider</param>
    /// <typeparam name="TProvider">The Type of Provider to be added to the StepBuilder</typeparam>
    /// <typeparam name="TContext">The Type of ProviderContext used to instatiate the Provider</typeparam>
    /// <returns>The StepBuilder</returns>
    public IStepBuilder AddProvider<TProvider, TContext>(TContext providerContext)
        where TProvider : IProvider<TContext>;

    /// <summary>
    /// Configures the step with the provided options 
    /// </summary>
    /// <param name="stepOptions"></param>
    /// <returns></returns>
    public IStepBuilder WithOptions(StepOptions stepOptions);
}

/// <summary>
/// Options used to configure a step
/// </summary>
public sealed class StepOptions
{
    /// <summary>
    /// If true, the user doesn't need to press a key for the search results to be shown
    /// </summary>
    public bool ShowSearchResultsAllways { get; set; } = false;
}