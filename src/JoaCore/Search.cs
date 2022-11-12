using JoaCore.Injectables;
using JoaCore.PluginCore;
using JoaCore.Settings;
using JoaCore.Step;
using JoaPluginsPackage;
using JoaPluginsPackage.Injectables;
using ExecutionContext = JoaCore.Step.ExecutionContext;

namespace JoaCore;

public class Search
{
    private readonly IJoaLogger _logger;
    private readonly PluginManager _pluginManager;
    private readonly SettingsManager _settingsManager;
    private readonly PluginServiceProvider _pluginServiceProvider;
    private readonly StepsManager _stepsManager;

    public Search(IJoaLogger logger, PluginManager pluginManager, SettingsManager settingsManager, PluginServiceProvider pluginServiceProvider, StepsManager stepsManager)
    {
        logger.Info(nameof(Search));
        _logger = logger;
        _pluginManager = pluginManager;
        _settingsManager = settingsManager;
        _pluginServiceProvider = pluginServiceProvider;
        _stepsManager = stepsManager;
    }

    public async Task ExecuteCommand(Guid resultId, string actionKey)
    {
        var pluginSearchResult = _stepsManager.GetCurrentStep().GetSearchResultFromId(resultId);

        var contextAction = GetContextAction(actionKey, pluginSearchResult);
        
        if (contextAction is null)
            return;

        var executionContext = new ExecutionContext
        {
            ContextAction = contextAction,
            ServiceProvider = _pluginServiceProvider.ServiceProvider
        };
        
        await Task.Run(() => pluginSearchResult.Execute(executionContext));

        if (executionContext.StepBuilder is null)
            return;
        
        _stepsManager.AddStep(executionContext.StepBuilder.Build());
    }
    
    //ToDo
    public async Task UpdateSearchResults(string searchString,
        Action<List<PluginSearchResult>> callback)
    {
        using var _ = _logger.TimedOperation(nameof(UpdateSearchResults));
        
        _logger.Info($"SearchString: ${searchString}");
        
        if (string.IsNullOrWhiteSpace(searchString))
        {
            callback(new List<PluginSearchResult>());    
            return;
        }

        callback(_stepsManager.GetCurrentStep().GetSearchResults(searchString).Take(8).ToList());
    }

    private ContextAction? GetContextAction(string actionKey, ISearchResult searchResult)
    {
        if (actionKey == "enter")
        {
            return new ContextAction
            {
                Key = "enter"
            };
        }

        return searchResult.Actions?.SingleOrDefault(x => x.Key == actionKey);
    }

}