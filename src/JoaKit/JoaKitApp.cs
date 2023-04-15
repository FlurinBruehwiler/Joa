using Modern.WindowKit.Platform;

namespace JoaKit;

public class JoaKitApp
{
    public IServiceProvider Services { get; set; }
    
    private readonly Dictionary<Type, IWindowImpl> _windows;
    internal static List<WindowManager> Managers = new();

    internal JoaKitApp(IServiceProvider services, Dictionary<Type, IWindowImpl> windows)
    {
        _windows = windows;
        Services = services;
    }

    public static JoaKitBuilder CreateBuilder()
    {
        return new JoaKitBuilder();
    }

    public void Run()
    {
        Managers = _windows.Select(x => new WindowManager(x.Value, x.Key, Services)).ToList();
            
        Parallel.ForEach(Managers, window =>
        {
            window.StartMainLoop();
        });
    }
}