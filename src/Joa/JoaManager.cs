using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using JoaInterface.PluginCore;
using JoaLauncher.Api.Injectables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JoaInterface;

public class JoaManager : IDisposable
{
    private readonly IOptions<PathsConfiguration> _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly IJoaLogger _joaLogger;
    public IServiceScope? CurrentScope { get; set; }
    private FileWatcher _fileWatcher;

    public JoaManager(IOptions<PathsConfiguration> configuration, IServiceProvider serviceProvider,
        IJoaLogger joaLogger)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _joaLogger = joaLogger;
        _fileWatcher = new FileWatcher(_configuration.Value.PluginLocation, NewScope, 500);
        NewScope();
    }

    public void NewScope()
    {
        _fileWatcher.Disable();
        var alcWeakRef = WaitForDisposal();

        if (alcWeakRef is not null)
        {
            for (var i = 0; alcWeakRef.IsAlive && i < 10; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            if (alcWeakRef.IsAlive)
                _joaLogger.Error("Unloading failed");
            else
                _joaLogger.Info("Unloading succeeded");
            
            _fileWatcher = new FileWatcher(_configuration.Value.PluginLocation, NewScope, 500);
        }

        CurrentScope = _serviceProvider.CreateScope();

        CurrentScope.ServiceProvider.GetService<Search>();
        _fileWatcher.Enable();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private WeakReference? WaitForDisposal()
    {
        if (CurrentScope is null)
            return null;

        var asmLoadContext = CurrentScope.ServiceProvider.GetRequiredService<PluginLoader>().AssemblyLoadContext;

        if (asmLoadContext is null)
            return null;

        var assembly = typeof(JsonSerializerOptions).Assembly;
        var updateHandlerType = assembly.GetType("System.Text.Json.JsonSerializerOptionsUpdateHandler");
        var clearCacheMethod = updateHandlerType?.GetMethod("ClearCache", BindingFlags.Static | BindingFlags.Public);
        clearCacheMethod?.Invoke(null, new object?[] { null });

        var alcWeakRef = new WeakReference(asmLoadContext);

        CurrentScope.Dispose();
        CurrentScope = null;

        asmLoadContext.Unload();
        return alcWeakRef;
    }

    public void Dispose()
    {
        CurrentScope?.Dispose();
    }
}