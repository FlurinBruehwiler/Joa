using JoaPluginsPackage.Plugin;

namespace JoaPluginsPackage.UI.Components;

public interface ISearchwindow
{
    public delegate Task NewInputDelegate(string searchString);
    public event NewInputDelegate NewInput;

    public delegate Task ItemSelectedDelegate(Guid pluginId, SearchResult searchResult);
    public event ItemSelectedDelegate ItemSelected;
    
    public void UpdateList(List<(SearchResult result, Guid pluginKey)> results);
}