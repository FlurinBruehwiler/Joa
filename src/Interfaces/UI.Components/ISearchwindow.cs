using Interfaces.Plugin;

namespace Interfaces.UI.Components;

public interface ISearchwindow
{
    public delegate Task NewInputDelegate(string searchString);
    public event NewInputDelegate NewInput;

    public delegate Task ItemSelectedDelegate(Guid pluginId, ICommand command);
    public event ItemSelectedDelegate ItemSelected;
    
    public void UpdateList(List<(ICommand result, Guid pluginKey)> results);
}