using Interfaces;
using Interfaces.Logger;
using JoaCore.PluginCore;
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
        PluginManager.ReloadPlugins();
    }

    public async Task ExecuteSearchResult(Guid pluginId, ISearchResult searchResult)
    {
        var pluginDef = PluginManager.Plugins?.First(p => p.Id == pluginId);
        if (pluginDef is null)
            return;
        await Task.Run(() => pluginDef.Plugin.Execute(searchResult)); //ToDo Check if it is really async
    }

    public async Task UpdateSearchResults(string searchString)
    {
        if (PluginManager.Plugins == null)
            return;
        
        var timer = JoaLogger.GetInstance().StartMeasure();
        
        SearchResults.Clear();

        var matchingPlugins = await GetMatchingPlugins(searchString);
        
        if (matchingPlugins.Count != 0)
        {
            matchingPlugins = PluginManager.Plugins.Where(x => x.StrictPlugin is not null).ToList();
        }
        
        var pluginsTasks = new Dictionary<Task<List<ISearchResult>>, Guid>();

        foreach (var pluginDef in matchingPlugins)
        {
            var pluginTask = Task.Run(() => pluginDef.Plugin.GetResults(searchString));
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

    private async Task<List<PluginDefinition>> GetMatchingPlugins(string searchString)
    {
        if (PluginManager.Plugins == null)
            return new List<PluginDefinition>();

        List<Task<(bool, Guid)>> validatorTasks = new();
        
        foreach (var pluginDef in PluginManager.Plugins)
        {
            if(pluginDef.StrictPlugin is null)
                continue;

            validatorTasks.Add(Task.Run(() => ValidatorWrapper(pluginDef, searchString)));
        }

        var results = await Task.WhenAll(validatorTasks);

        return results.Select(result =>
            PluginManager.Plugins.First(plugin => plugin.Id == result.Item2)).ToList();
    }

    private async Task<(bool, Guid)> ValidatorWrapper(PluginDefinition pluginDef, string searchString)
    {
        return (await Task.Run(() => pluginDef.StrictPlugin!.Validator(searchString)), pluginDef.Id);
    }
}