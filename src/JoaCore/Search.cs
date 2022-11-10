using JoaPluginsPackage;

namespace JoaCore;

public class Search
{
    private readonly PluginManager _pluginManager;
    private readonly PluginServiceProvider _pluginServiceProvider;
    private readonly Queue<Step> _steps; 

    public Search(PluginManager pluginManager, PluginServiceProvider pluginServiceProvider)
    { 
        _pluginManager = pluginManager;
        _pluginServiceProvider = pluginServiceProvider;
        _pluginManager.ReloadPlugins();
        _steps = new Queue<Step>();
        _steps.Enqueue(new Step
        {
            Providers = pluginManager.GlobalProviders.Where(x => x.Condition is null)
                .Select(x => x.Provider).ToList()
        });
    }

    public async Task ExecuteCommand(Guid resultId, string actionKey)
    {
        var pluginSearchResult = _steps.Peek().GetSearchResultFromId(resultId);

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
        
        _steps.Enqueue(executionContext.StepBuilder.Build());
    }
    
    //ToDo
    public async Task UpdateSearchResults(string searchString, Guid stepId,
        Action<List<PluginSearchResult>> callback)
    {
        while (true)
        {
            if (_steps.Peek().StepId == stepId)
            {
                callback(_steps.Peek().GetSearchResults(searchString));
                return;
            }

            _steps.Dequeue();
        }
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