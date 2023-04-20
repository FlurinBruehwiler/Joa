using Microsoft.Extensions.DependencyInjection;
using Modern.WindowKit.Platform;
using Modern.WindowKit.Threading;

namespace JoaKit;

public class JoaKitApp
{
    private readonly List<WindowDefinition> _windows;
    internal static List<WindowManager> WindowManagers = new();
    internal IWindowImpl? CurrentlyBuildingWindow = null;
    private IServiceScope? _serviceScope;
    public IServiceProvider Services { get; set; }

    internal JoaKitApp(IServiceCollection services, List<WindowDefinition> windows)
    {
        _windows = windows;
        
        services.AddSingleton<IWindowImpl>(_ =>
        {
            if (CurrentlyBuildingWindow is null)
                throw new Exception("No window is currently being built, can't inject window");

            return CurrentlyBuildingWindow;
        });
        services.AddSingleton(this);
        
        Services = services.BuildServiceProvider();
        RenewScope();
    }

    public void RenewScope()
    {
        _serviceScope?.Dispose();
        _serviceScope = Services.CreateScope();
        Services = _serviceScope.ServiceProvider;
    }

    public static JoaKitBuilder CreateBuilder()
    {
        return new JoaKitBuilder();
    }

    public void Run()
    {
        var cancellationTokenSource = new CancellationTokenSource();

        WindowManagers = _windows.Select(x => 
            new WindowManager(this, x.WindowImpl, x.RootComponent, cancellationTokenSource)).ToList();
        
        Dispatcher.UIThread.MainLoop(cancellationTokenSource.Token);
    }
}