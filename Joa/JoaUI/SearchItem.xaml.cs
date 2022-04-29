using System.Windows.Controls;
using Interfaces;

namespace JoaUI;

public partial class SearchItem : UserControl
{
    public SearchItem(ISearchResult result)
    {
        InitializeComponent();
        CaptLabel.Content = result.Caption;
        DescLabel.Content = result.Description;
    }
}

public static class ResultTranslator
{
    public static SearchItem Visualize(this ISearchResult searchResult) => new SearchItem(searchResult);
}