using JoaCore.PluginCore;
using JoaPluginsPackage;

namespace JoaCore.Step;

public class Step
{
    public required List<ProviderWrapper> Providers { get; set; }

    public Guid StepId { get; set; } = Guid.NewGuid();
    
    private List<PluginSearchResult> _lastSearchResults;

    public Step()
    {
        _lastSearchResults = new List<PluginSearchResult>();
    }

    public ISearchResult GetSearchResultFromId(Guid id)
    {
        return _lastSearchResults.FirstOrDefault(x => x.CommandId == id)?.SearchResult 
               ?? throw new Exception("Could not find Search Result");
    }
    
    public List<PluginSearchResult> GetSearchResults(string searchString)
    {
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