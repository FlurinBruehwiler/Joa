using Joa.Api.Providers;

namespace Joa.Api;

/// <summary>
/// Is used to Build a new Step. A Step Containt Providers
/// </summary>
public interface IStepBuilder
{
    /// <summary>
    /// Can be used to add a Provider to the Step.
    /// Extra Parameters used to instantiate the provider can be passed in as arguments.
    /// </summary>
    /// <param name="parameter"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IStepBuilder AddProvider<T>(params object[] parameter) where T : IProvider;
}