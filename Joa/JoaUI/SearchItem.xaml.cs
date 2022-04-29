using System.Windows.Controls;
using PluginBase;

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