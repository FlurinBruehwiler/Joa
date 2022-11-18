using JoaInterface.PluginCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JoaInterface;

public class JoaManager : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    public IServiceScope? CurrentScope { get; set; }
    private readonly FileWatcher _fileWatcher;
    
    public JoaManager(IOptions<PathsConfiguration> configuration, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _fileWatcher = new FileWatcher(configuration.Value.PluginLocation, NewScope, 500);
        NewScope();
    }

    public void NewScope()
    {
        _fileWatcher.Disable();
        WaitForDisposal();
        
        CurrentScope = _serviceProvider.CreateScope(); 
        
        CurrentScope.ServiceProvider.GetService<Search>();
        _fileWatcher.Enable();
    }

    private void WaitForDisposal()
    {
        if (CurrentScope is null)
            return;

        var asmLoadContext = CurrentScope.ServiceProvider.GetRequiredService<PluginLoader>().AssemblyLoadContext;

        if (asmLoadContext is null)
            return;
        
        var alcWeakRef = new WeakReference(asmLoadContext);
        
        CurrentScope.Dispose();
        CurrentScope = null;
        
        asmLoadContext.Unload();
        asmLoadContext = null;

        for (var i = 0; alcWeakRef.IsAlive && i < 10; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public void Dispose()
    {
        CurrentScope?.Dispose();
    }
}