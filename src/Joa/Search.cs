using JoaInterface.Hubs;
using JoaInterface.PluginCore;
using JoaInterface.Step;
using JoaPluginsPackage;
using JoaPluginsPackage.Injectables;
using Microsoft.AspNetCore.SignalR;
using ExecutionContext = JoaInterface.Step.ExecutionContext;

namespace JoaInterface;

public class Search
{
    private readonly IJoaLogger _logger;
    private readonly PluginServiceProvider _pluginServiceProvider;
    private readonly StepsManager _stepsManager;
    private readonly IHubContext<SearchHub> _hubContext;

    public Search(IJoaLogger logger, PluginServiceProvider pluginServiceProvider, StepsManager stepsManager,
        IHubContext<SearchHub> hubContext)
    {
        logger.Info(nameof(Search));
        _logger = logger;
        _pluginServiceProvider = pluginServiceProvider;
        _stepsManager = stepsManager;
        _hubContext = hubContext;
    }

    public async Task ExecuteCommand(Guid resultId, string actionKey)
    {
        var pluginSearchResult = _stepsManager.GetCurrentStep().GetSearchResultFromId(resultId);

        var contextAction = GetContextAction(actionKey, pluginSearchResult);

        if (contextAction is null)
            return;

        var executionContext = new ExecutionContext(pluginSearchResult)
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
    public async Task UpdateSearchResults(string searchString)
    {
        using var _ = _logger.TimedOperation(nameof(UpdateSearchResults));

        _logger.Info($"SearchString: ${searchString}");

        List<PluginSearchResult> results;

        if (string.IsNullOrWhiteSpace(searchString))
        {
            results = new List<PluginSearchResult>();
        }
        else
        {
            results = _stepsManager.GetCurrentStep().GetSearchResults(searchString).Take(8).ToList();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveSearchResults", searchString, results);
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