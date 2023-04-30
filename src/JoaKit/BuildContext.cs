using Microsoft.Extensions.DependencyInjection;

namespace JoaKit;

public class BuildContext
{
    private readonly IServiceProvider _serviceProvider;

    public BuildContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private readonly Dictionary<ComponentHash, Component> _components = new();

    public Component GetComponent(ComponentHash componentHash, Type componentType, Renderer renderer)
    {
        if (_components.TryGetValue(componentHash, out var component))
        {
            return component;
        }

        component = (Component)ActivatorUtilities.CreateInstance(_serviceProvider, componentType);
        component.Renderer = renderer;
        
        _components.Add(componentHash, component);
        
        return component;
    }
}

public record struct ComponentHash(string? Key, int LineNumber, string FilePath);