using System;
using JoaPluginsPackage.Plugin;

namespace JoaUI;

public partial class SearchItem
{
    public (Guid pluginId, ICommand searchResult) Result { get; }
    public SearchItem(ICommand result, Guid pluginId)
    {
        Result = (pluginId, result);
        InitializeComponent();
        CaptLabel.Content = result.Caption;
        DescLabel.Content = result.Description;
    }
}

public static class ResultTranslator
{
    public static SearchItem Visualize(this ICommand command, Guid plugin) => new SearchItem(command, plugin);
}