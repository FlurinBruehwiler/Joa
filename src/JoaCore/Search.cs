using JoaCore.PluginCore;
using JoaCore.Step;
using JoaPluginsPackage;
using ExecutionContext = JoaCore.Step.ExecutionContext;

namespace JoaCore;

public class Search
{
    private readonly PluginManager _pluginManager;
    private readonly PluginServiceProvider _pluginServiceProvider;
    private readonly StepsManager _stepsManager;

    public Search(PluginManager pluginManager, PluginServiceProvider pluginServiceProvider, StepsManager stepsManager)
    { 
        _pluginManager = pluginManager;
        _pluginServiceProvider = pluginServiceProvider;
        _stepsManager = stepsManager;
        _pluginManager.ReloadPlugins();

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
        callback(_stepsManager.GetCurrentStep().GetSearchResults(searchString));
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