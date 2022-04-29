using System.Windows;
using System.Windows.Controls;
using PluginBase;

namespace JoaUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateList(object sender, ISearchResult result)
        {
            ResultList.Items.Add(new SearchItem(result));
        }
        
        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}