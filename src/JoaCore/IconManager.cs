using JoaCore.PluginCore;
using JoaPluginsPackage.Injectables;
using Microsoft.AspNetCore.SignalR;

namespace JoaCore;

public class IconManager
{
    private readonly IIconHelper _iconHelper;
    private readonly IHubContext _iHubContext;
    private readonly List<string> _iconsCache;

    public IconManager(IIconHelper iconHelper, IHubContext iHubContext)
    {
        _iconHelper = iconHelper;
        _iHubContext = iHubContext;
        _iconsCache = new List<string>();
    }
    
    public void UpdateIcons(List<PluginDefinition>? plugins)
    {
        if (plugins is null)
            throw new Exception(); //ToDo

        List<string> newIcons = new();

        foreach (var pluginDefinition in plugins)
        {
            var iconDir = _iconHelper.GetIconsDirectoryIfExists(pluginDefinition.Plugin.GetType());
            
            if(iconDir is null)
                continue;

            var icons = Directory.GetFiles(iconDir);

            var newIconsOfPlugin = icons.Except(_iconsCache).ToList();
            
            _iconsCache.AddRange(newIconsOfPlugin);
            newIcons.AddRange(newIconsOfPlugin);
        }

        _iHubContext.Clients.All.SendAsync("receiveLoadImages", newIcons);
    }
}