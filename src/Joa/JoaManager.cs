using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Joa.PluginCore;
using JoaKit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Joa;

public class JoaManager
{
    public Func<bool, Task>? ShowUi { get; set; }

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<JoaManager> _logger;
    private readonly FileSystemManager _fileSystemManager;
    private readonly JoaKitApp _joaKitApp;
    private FileWatcher _fileWatcher;
    private const string AssemblyType = "System.Text.Json.JsonSerializerOptionsUpdateHandler";
    private const string ClearCache = "ClearCache";

    public JoaManager(IServiceProvider serviceProvider, ILogger<JoaManager> logger, FileSystemManager fileSystemManager, JoaKitApp joaKitApp)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _fileSystemManager = fileSystemManager;
        _joaKitApp = joaKitApp;
        _fileWatcher = new FileWatcher(fileSystemManager.GetPluginsLocation(), NewScope, 500);
    }

    private void NewScope()
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
                _logger.LogError("Unloading failed");
            else
            {
                _logger.LogInformation("Unloading succeeded");
                Debugger.Break();
            }

            _fileWatcher = new FileWatcher(_fileSystemManager.GetPluginsLocation(), NewScope, 500);
        }

        _serviceProvider.GetRequiredService<Search>();

        _fileWatcher.Enable();

        ShowUi?.Invoke(true).GetAwaiter().GetResult();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private WeakReference? WaitForDisposal()
    {
        var asmLoadContext = _serviceProvider.GetRequiredService<PluginLoader>().AssemblyLoadContext;

        if (asmLoadContext is null)
            return null;

        var assembly = typeof(JsonSerializerOptions).Assembly;
        var updateHandlerType = assembly.GetType(AssemblyType);
        var clearCacheMethod = updateHandlerType?.GetMethod(ClearCache, BindingFlags.Static | BindingFlags.Public);
        clearCacheMethod?.Invoke(null, new object?[] { null });

        var alcWeakRef = new WeakReference(asmLoadContext);

        _joaKitApp.RenewScope();

        ShowUi?.Invoke(false).GetAwaiter().GetResult();

        asmLoadContext.Unload();
        return alcWeakRef;
    }
}