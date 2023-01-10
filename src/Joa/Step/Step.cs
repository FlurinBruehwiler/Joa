using Joa.PluginCore;
using JoaLauncher.Api;

namespace Joa.Step;

public class Step
{
    public required List<ProviderWrapper> Providers { get; init; }

    public required string Name { get; init; }
    public required StepOptions Options { get; init; }
    public Guid Id { get; set; } = Guid.NewGuid();

    private List<PluginSearchResult> _lastSearchResults;

    public Step()
    {
        _lastSearchResults = new List<PluginSearchResult>();
    }


    public List<PluginSearchResult> GetSearchResults(string searchString)
    {
        if (!Options.ShowSearchResultsAllways && string.IsNullOrWhiteSpace(searchString))
            return new List<PluginSearchResult>();

        var matchingProvider = Providers.FirstOrDefault(x => x.Condition is not null && x.Condition(searchString));

        if (matchingProvider is not null)
        {
            _lastSearchResults = matchingProvider.Provider
                .GetSearchResults(searchString)
                .ToPluginSerachResults();
        }
        else
        {
            _lastSearchResults = Providers
                .SelectMany(x => x.Provider
                    .GetSearchResults(searchString))
                .ToList()
                .ToPluginSerachResults()
                .Sort(searchString);
        }

        return _lastSearchResults;
    }
}