using System;
using System.Windows.Controls;
using Interfaces;

namespace JoaUI;

public partial class SearchItem : UserControl
{
    public (Guid pluginId, ISearchResult searchResult) Result { get; }
    public SearchItem(ISearchResult result, Guid pluginId)
    {
        Result = (pluginId, result);
        InitializeComponent();
        CaptLabel.Content = result.Caption;
        DescLabel.Content = result.Description;
    }
}

public static class ResultTranslator
{
    public static SearchItem Visualize(this ISearchResult searchResult, Guid plugin) => new SearchItem(searchResult, plugin);
}