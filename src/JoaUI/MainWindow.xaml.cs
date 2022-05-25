using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Interfaces;
using Interfaces.UI.Components;


namespace JoaUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ISearchwindow
    { 
        public MainWindow()
        {
            InitializeComponent();
        }

        public event ISearchwindow.NewInputDelegate? NewInput;
        public event ISearchwindow.ItemSelectedDelegate? ItemSelected;

        public void UpdateList(List<(ISearchResult result, Guid pluginKey)> results)
        {
            ResultList.Items.Clear();
            foreach (var searchItem in results.Select(searchResult => searchResult.result.Visualize(searchResult.pluginKey)))
            {
                ResultList.Items.Add(searchItem);
            }
            Searchbar.Height = ResultList.Items.Count * 60;
        }
        
        private void TextModified(object sender, TextChangedEventArgs e)
        {
            ResultList.Items.Clear();
            NewInput?.Invoke(Box.Text);
        }

        private void KeyPressedOnItem(object sender, KeyEventArgs e)
        {
            if (sender is not ListView {SelectedItem: SearchItem i}) return;
            ItemSelected?.Invoke(i.Result.pluginId, i.Result.searchResult);
        }
    }
}