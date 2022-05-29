using Interfaces;
using Interfaces.Logger;
using JoaCore.Settings;
using Microsoft.Extensions.Configuration;

namespace JoaCore;

public class Search
{
    public delegate void ResultsUpdatedDelegate(List<(ISearchResult, Guid)> results);
    public event ResultsUpdatedDelegate? ResultsUpdated;
    private SettingsManager SettingsManager { get; set; }
    private List<(ISearchResult, Guid)> SearchResults { get; }
    private PluginManager PluginManager { get; set; }

    public Search(IConfiguration configuration)
    {
        SearchResults = new List<(ISearchResult, Guid)>();
        SettingsManager = new SettingsManager(new CoreSettings(), configuration);
        PluginManager = new PluginManager(SettingsManager, configuration);
    }

    public async Task ExecuteSearchResult(Guid pluginId, ISearchResult searchResult)
    {
        var pluginDef = PluginManager.Plugins.First(p => p.Id == pluginId);
        await Task.Run(() => pluginDef.Plugin.Execute(searchResult)); //ToDo Check if it is really async
    }

    public async Task UpdateSearchResults(string searchString)
    {
        var timer = JoaLogger.GetInstance().StartMeasure();
        
        SearchResults.Clear();
        var pluginsTasks = new Dictionary<Task<List<ISearchResult>>, Guid>();

        foreach (var pluginDef in PluginManager.Plugins)
        {
            var plugin = pluginDef.Plugin;
            var pluginTask = Task.Run(() => plugin.GetResults(searchString));
            pluginsTasks.Add(pluginTask, pluginDef.Id);
        }

        while (pluginsTasks.Count > 0)
        {
            var pluginTask = await Task.WhenAny(pluginsTasks.Keys);

            try
            {
                var pluginResult = await pluginTask;
                foreach (var searchResult in pluginResult)
                {
                    SearchResults.Add((searchResult, pluginsTasks[pluginTask]));
                }
            }
            catch (Exception e)
            {
                JoaLogger.GetInstance().Log(
                    $"There was an exception during the execution of a plugin with the search term \"{searchString}\" with the following exception{Environment.NewLine}" 
                    + e, IJoaLogger.LogLevel.Error);
            }
            ResultsUpdated?.Invoke(SearchResults);
            pluginsTasks.Remove(pluginTask);
        }
        
        JoaLogger.GetInstance().LogMeasureResult(timer,$"{nameof(UpdateSearchResults)}:{searchString}");
    }
}