using Microsoft.Extensions.DependencyInjection;

namespace JoaKit;

public class BuildContext
{
    private readonly IServiceProvider _serviceProvider;

    public BuildContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private readonly Dictionary<ComponentHash, IComponent> _components = new();

    public IComponent GetComponent(ComponentHash componentHash, Type componentType)
    {
        if (_components.TryGetValue(componentHash, out var component))
        {
            return component;
        }

        return (IComponent)ActivatorUtilities.CreateInstance(_serviceProvider, componentType);
    }
}

public record struct ComponentHash(string? Key, int LineNumber, string FilePath);