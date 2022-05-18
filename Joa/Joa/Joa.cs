using System.Windows;
using Interfaces;
using Interfaces.Logger;
using Interfaces.UI.Components;
using JoaCore;
using JoaUI;
using Microsoft.Extensions.Logging;

namespace Joa;

class Joa
{
    private static Search _search;

    [STAThread]
    public static void Main()
    {
        _search = new Search();
        InitWindow(new MainWindow());
        LoggingManager.JoaLogger.Log("Init Complete", IJoaLogger.LogLevel.Info);
    }

    private static void InitWindow(ISearchwindow searchwindow)
    {
        Connect(searchwindow);
        if (searchwindow is not Window window) return;
        
        var application = new Application();
        application.Run(window);
    }
    
    private static void Connect(ISearchwindow searchwindow)
    {
        _search.ResultsUpdated += searchwindow.UpdateList;
        searchwindow.NewInput += ActivateSearch;
        searchwindow.ItemSelected += ActivateExecute;
    }
    
    private static async Task ActivateSearch(string searchString)
    {
        await _search.UpdateSearchResults(searchString);
    }

    private static async Task ActivateExecute(Guid pluginId, ISearchResult searchResult)
    {
        await _search.ExecuteSearchResult(pluginId, searchResult);
    }
}




    

    
