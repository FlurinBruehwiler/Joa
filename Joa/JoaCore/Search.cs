using System.Reflection;
using Interfaces;
using Interfaces.Logger;
using Interfaces.Settings.Attributes;
using JoaCore.PluginCore;
using JoaCore.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JoaCore;

public class Search
{
    private readonly IConfiguration _configuration;
    private readonly PluginLoader _pluginLoader;
    private readonly CoreSettings _coreSettings;
    private readonly ILogger<IJoaLogger> _logger;
    
    public delegate void ResultsUpdatedDelegate(List<(ISearchResult, Guid)> results);
    public event ResultsUpdatedDelegate? ResultsUpdated;

    public SettingsManager SettingsManager { get; set; } = null!;
    private List<PluginDefinition> Plugins { get; set; } = null!;
    private List<(ISearchResult, Guid)> SearchResults { get; }

    public Search(IConfiguration configuration)
    {
        _configuration = configuration;
        SearchResults = new List<(ISearchResult, Guid)>();
        _pluginLoader = new PluginLoader(_configuration);
        _coreSettings = new CoreSettings();
        _logger = new Logger<IJoaLogger>(new LoggerFactory());
        ReloadSearch();
    }

    public void ReloadSearch()
    {
        var timer = JoaLogger.GetInstance().StartMeasure();
        
        Plugins = new List<PluginDefinition>();
        foreach (var plugin in _pluginLoader.InstantiatePlugins(_coreSettings).ToList())
        {
            Plugins.Add(new PluginDefinition(plugin, GetPluginInfos(plugin.GetType())));
        }
        SettingsManager = new SettingsManager(_coreSettings, Plugins, _configuration);
        
        JoaLogger.GetInstance().LogMeasureResult(timer, nameof(ReloadSearch));
    }
    
    private PluginAttribute GetPluginInfos(MemberInfo pluginType)
    {
        var attr = Attribute.GetCustomAttributes(pluginType).FirstOrDefault();

        if (attr is not PluginAttribute pluginAttribute)
            return new PluginAttribute(pluginType.Name, string.Empty);
        
        return pluginAttribute;
    }
    
    public async Task ExecuteSearchResult(Guid pluginId, ISearchResult searchResult)
    {
        var pluginDef = Plugins.First(p => p.Id == pluginId);
        await Task.Run(() => pluginDef.Plugin.Execute(searchResult)); //ToDo Check if it is really async
    }

    public async Task UpdateSearchResults(string searchString)
    {
        var timer = JoaLogger.GetInstance().StartMeasure();
        
        SearchResults.Clear();
        var pluginsTasks = new Dictionary<Task<List<ISearchResult>>, Guid>();

        foreach (var pluginDef in Plugins)
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