using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modern.WindowKit;
using Modern.WindowKit.Platform;

namespace JoaKit;

public class JoaKitBuilder
{
    private Dictionary<Type, IWindowImpl> _windows = new();

    public JoaKitBuilder()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }
    
    public JoaKitBuilder AddWindow<T>(Action<IWindowImpl> configureWindow) where T : UiComponent
    {
        var window = AvaloniaGlobals.GetRequiredService<IWindowingPlatform>().CreateWindow();
        configureWindow(window);
        _windows.Add(typeof(T), window);
        return this;
    }

    public JoaKitApp Build()
    {
        return new JoaKitApp(Services.BuildServiceProvider(), _windows);
    }

    public IConfiguration Configuration { get; private set; }
    public IServiceCollection Services { get; } = new ServiceCollection();
}