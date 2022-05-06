using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using JoaCore;
using Interfaces;

namespace JoaUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Search _search;
        private readonly PluginLoader _loader;

        private delegate Task NewInputDelegate(string searchString);

        private event NewInputDelegate NewInput;

        public MainWindow()
        {
            NewInput += ActivateSearch;
            _search = new Search();
            _search.ResultsUpdated += UpdateList;
            InitializeComponent();
        }

        private void UpdateList(List<ISearchResult> results)
        {
            ResultList.Items.Clear();
            foreach (var searchResult in results)
            {
                ResultList.Items.Add(searchResult.Visualize());
            }

            Searchbar.Height = ResultList.Items.Count * 60;
        }

        private void TextModified(object sender, TextChangedEventArgs e)
        {
            ResultList.Items.Clear();
            NewInput?.Invoke(Box.Text);
        }

        private async Task ActivateSearch(string searchString)
        {
            await _search.UpdateSearchResults(searchString);
        }
    }
}