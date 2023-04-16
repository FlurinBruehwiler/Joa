using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modern.WindowKit;
using Modern.WindowKit.Platform;

namespace JoaKit;

public class JoaKitBuilder
{
    private List<WindowDefinition> _windows = new();

    public JoaKitBuilder()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }
    
    public JoaKitBuilder AddWindow<T>(Action<IWindowImpl> configureWindow) where T : IComponent
    {
        var window = AvaloniaGlobals.GetRequiredService<IWindowingPlatform>().CreateWindow();
        configureWindow(window);
        _windows.Add(new WindowDefinition(typeof(T), window));
        return this;
    }
    
    public JoaKitBuilder AddWindow<T>() where T : IComponent
    {
        AddWindow<T>(_ => {});
        return this;
    }

    public JoaKitApp Build()
    {
        return new JoaKitApp(Services.BuildServiceProvider(), _windows);
    }

    public IConfiguration Configuration { get; private set; }
    public IServiceCollection Services { get; } = new ServiceCollection();
}

public record WindowDefinition(Type RootComponent, IWindowImpl WindowImpl);