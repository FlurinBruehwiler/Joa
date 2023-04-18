using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Joa.PluginCore;
using JoaLauncher.Api.Injectables;
using Microsoft.Extensions.DependencyInjection;

namespace Joa;

public class JoaManager : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IJoaLogger _joaLogger;
    private readonly FileSystemManager _fileSystemManager;
    public IServiceScope? CurrentScope { get; private set; }
    private FileWatcher _fileWatcher;

    private const string AssemblyType = "System.Text.Json.JsonSerializerOptionsUpdateHandler";
    private const string ClearCache = "ClearCache";

    public Func<bool, Task>? ShowUi { get; set; }
    // public Func<Action, Task> ExecuteOnUiThread { get; set; }

    public JoaManager(IServiceProvider serviceProvider, IJoaLogger joaLogger, FileSystemManager fileSystemManager)
    {
        _serviceProvider = serviceProvider;
        _joaLogger = joaLogger;
        _fileSystemManager = fileSystemManager;
        _fileWatcher = new FileWatcher(fileSystemManager.GetPluginsLocation(), NewScope, 500);
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
            {
                _joaLogger.Info("Unloading succeeded");
                Debugger.Break();                
            }

            _fileWatcher = new FileWatcher(_fileSystemManager.GetPluginsLocation(), NewScope, 500);
        }

        CurrentScope = _serviceProvider.CreateScope();

        CurrentScope.ServiceProvider.GetService<Search>();

        _fileWatcher.Enable();

        ShowUi?.Invoke(true).GetAwaiter().GetResult();
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
        var updateHandlerType = assembly.GetType(AssemblyType);
        var clearCacheMethod = updateHandlerType?.GetMethod(ClearCache, BindingFlags.Static | BindingFlags.Public);
        clearCacheMethod?.Invoke(null, new object?[] { null });

        var alcWeakRef = new WeakReference(asmLoadContext);

        CurrentScope.Dispose();
        CurrentScope = null;

        ShowUi?.Invoke(false).GetAwaiter().GetResult();

        asmLoadContext.Unload();
        return alcWeakRef;
    }



    public void Dispose()
    {
        CurrentScope?.Dispose();
    }
}