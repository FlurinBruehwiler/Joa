using Microsoft.Extensions.DependencyInjection;
using Modern.WindowKit;
using Modern.WindowKit.Platform;
using Modern.WindowKit.Threading;

namespace JoaKit;

public class JoaKitApp
{
    internal static readonly List<WindowManager> WindowManagers = new();
    internal IWindowImpl? CurrentlyBuildingWindow = null;
    private IServiceScope? _serviceScope;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    public IServiceProvider Services { get; private set; }

    internal JoaKitApp(IServiceCollection services)
    {
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

    public void CreateWindow<T>(Action<IWindowImpl> configure, bool show = true) where T : Component
    {
        var window = AvaloniaGlobals.GetRequiredService<IWindowingPlatform>().CreateWindow();
        configure(window);
        WindowManagers.Add(new WindowManager(this, window, typeof(T), _cancellationTokenSource));
        if (show)
        {
            window.Show(true, false);
        }
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
        JoaSynchronizationContext.Install();
        
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            File.WriteAllText("crash.log", args.ExceptionObject.ToString());
        };

        Dispatcher.UIThread.MainLoop(_cancellationTokenSource.Token);
    }
}