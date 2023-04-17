using Microsoft.Extensions.DependencyInjection;
using Modern.WindowKit.Threading;

namespace JoaKit;

public class JoaKitApp
{
    public IServiceProvider Services { get; set; }

    private readonly IServiceCollection _serviceCollection;
    private readonly List<WindowDefinition> _windows;
    internal static List<WindowManager> WindowManagers = new();

    internal JoaKitApp(IServiceProvider services, IServiceCollection serviceCollection, List<WindowDefinition> windows)
    {
        _serviceCollection = serviceCollection;
        _windows = windows;
        Services = services;
    }

    public static JoaKitBuilder CreateBuilder()
    {
        return new JoaKitBuilder();
    }

    public void Run()
    {
        var cancellationTokenSource = new CancellationTokenSource();

        WindowManagers = _windows.Select(x => 
            new WindowManager(x.WindowImpl, x.RootComponent, _serviceCollection, cancellationTokenSource)).ToList();
        
        Dispatcher.UIThread.MainLoop(cancellationTokenSource.Token);
    }
}