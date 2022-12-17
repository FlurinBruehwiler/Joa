using Joa.Hubs;
using Joa.PluginCore;
using Joa.Settings;
using Joa.Step;
using JoaLauncher.Api;
using JoaLauncher.Api.Enums;
using JoaLauncher.Api.Injectables;
using Microsoft.AspNetCore.SignalR;
using ExecutionContext = Joa.Step.ExecutionContext;

namespace Joa;

public class Search
{
    private readonly IJoaLogger _logger;
    private readonly PluginServiceProvider _pluginServiceProvider;
    private readonly StepsManager _stepsManager;
    private readonly IHubContext<SearchHub> _hubContext;
    private readonly SettingsManager _settingsManager;

    public Search(IJoaLogger logger, PluginServiceProvider pluginServiceProvider, StepsManager stepsManager,
        IHubContext<SearchHub> hubContext, SettingsManager settingsManager)
    {
        _logger = logger;
        _pluginServiceProvider = pluginServiceProvider;
        _stepsManager = stepsManager;
        _hubContext = hubContext;
        _settingsManager = settingsManager;
    }

    public async Task ExecuteCommand(Guid resultId, string actionId)
    {
        var pluginSearchResult = _stepsManager.GetCurrentStep().GetSearchResultFromId(resultId);

        var contextAction = GetContextAction(actionId, pluginSearchResult);

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
        
        foreach (var result in results)
        {
            result.SearchResult.Actions ??= new List<ContextAction>();

            if (result.SearchResult.Actions.All(x => x.Key != Key.Enter))
            {
                result.SearchResult.Actions.Add(new ContextAction
                {
                    Id = "Enter",
                    Key = Key.Enter,
                    Name = "Execute"
                });
            }
        }

        await _hubContext.Clients.All.SendAsync("ReceiveSearchResults", searchString, results);
    }

    private ContextAction? GetContextAction(string actionId, SearchResult searchResult)
    {
        return searchResult.Actions?.SingleOrDefault(x => x.Id == actionId);
    }
}