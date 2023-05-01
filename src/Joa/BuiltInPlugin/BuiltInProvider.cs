using Joa.PluginCore;
using Joa.UI.Settings;
using JoaKit;
using JoaLauncher.Api;
using JoaLauncher.Api.Providers;
using Microsoft.Extensions.DependencyInjection;
using Modern.WindowKit;
using Modern.WindowKit.Threading;
using SkiaSharp;

namespace Joa.BuiltInPlugin;

public class BuiltInProvider : IProvider
{
    private readonly List<SearchResult> _searchResults;

    public BuiltInProvider(IServiceProvider serviceProvider, PluginManager pluginManager)
    {
        _searchResults = new List<SearchResult>
        {
            new BuiltInSearchResult
            {
                Title = "Settings",
                Description = "Change Settings",
                Icon = string.Empty,
                ExecutionAction = _ =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        serviceProvider.GetRequiredService<JoaKitApp>().CreateWindow<SettingsWindow>(
                            window =>
                            {
                                window.Resize(new Size(800, 600));
                                window.SetTitle("Joa Settings");
                                window.SetIcon(SKBitmap.Decode("Assets/icon.png"));
                            });

                    });
                }
            },
            new BuiltInSearchResult
            {
                Title = "Refresh indexes",
                Description = "Refresh all indexes of all plugins",
                Icon = string.Empty,
                ExecutionAction = _ => Task.Run(pluginManager.UpdateIndexesAsync)
            }
        };
    }

    public List<SearchResult> GetSearchResults(string searchString)
    {
        return _searchResults;
    }
}