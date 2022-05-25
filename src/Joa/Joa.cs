using System.Windows;
using Interfaces;
using Interfaces.Logger;
using Interfaces.UI.Components;
using JoaCore;
using JoaUI;
using Microsoft.Extensions.Configuration;


namespace Joa;

class Joa
{
    private static Search _search = null!;

    [STAThread]
    public static void Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
        _search = new Search(configuration);
        InitWindow(new MainWindow());
        JoaLogger.GetInstance().Log("Init Complete", IJoaLogger.LogLevel.Info);
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




    

    
