using JoaCore;
using JoaPluginsPackage.Plugin;
using Microsoft.Extensions.Hosting;

namespace JoaInterface;

public class UiManagement : IHostedService
{
    private readonly PluginManager _pluginManager;
    private readonly JoaLogger _logger;

    public UiManagement(PluginManager pluginManager, JoaLogger logger)
    {
        _pluginManager = pluginManager;
        _logger = logger;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {

        _pluginManager.GetPluginsOfType<IUiPlugin>().ForEach(x =>
        {
            try
            {
                x.Start("");
            }
            catch (Exception)
            {
                _logger.Error($"Error while trying to start the following UI Plugin {x.GetType().Assembly.Location}");
            }
        });
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _pluginManager.GetPluginsOfType<IUiPlugin>().ForEach(x =>
        {
            try
            {
                x.Stop();
            }
            catch (Exception)
            {
                _logger.Error($"Error while trying to start the following UI Plugin {x.GetType().Assembly.Location}");
            }
        });
        
        return Task.CompletedTask;
    }
}