using Joa.PluginCore;
using Joa.Settings;
using Joa.Steps;
using JoaLauncher.Api;
using JoaLauncher.Api.Enums;
using Microsoft.Extensions.Logging;
using ExecutionContext = Joa.Steps.ExecutionContext;

namespace Joa;

public class Search
{
    private readonly ILogger<Search> _logger;
    private readonly PluginServiceProvider _pluginServiceProvider;
    private readonly SettingsManager _settingsManager;

    public Search(ILogger<Search> logger, PluginServiceProvider pluginServiceProvider, SettingsManager settingsManager)
    {
        _logger = logger;
        _pluginServiceProvider = pluginServiceProvider;
        _settingsManager = settingsManager;
    }

    public async Task<Step?> ExecuteCommand(SearchResult searchResult, ContextAction contextAction)
    {
        var executionContext = new ExecutionContext(searchResult)
        {
            ContextAction = contextAction,
            ServiceProvider = _pluginServiceProvider.ServiceProvider
        };

        await Task.Run(() => searchResult.Execute(executionContext));

        return executionContext.StepBuilder?.Build();
    }

    public List<PluginSearchResult> UpdateSearchResults(Step step, string searchString)
    {
        using var _ = _logger.TimedLogOperation();

        _logger.LogInformation("SearchString: {searchString}", searchString);

        var results = step.GetSearchResults(searchString).Take(8).ToList();

        _logger.LogInformation(Environment.CurrentManagedThreadId.ToString());
        
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

        return results;
    }
}