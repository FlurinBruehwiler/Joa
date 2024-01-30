using FlurinBruehwiler.Helpers;
using JoaLauncher.Api;
using JoaLauncher.Api.Injectables;
using JoaLauncher.Api.Providers;
using Octokit;

namespace Github.Providers;

public class RepositoryProvider : IProvider<Repository>
{
    private readonly Repository _repository;
    private readonly IBrowserHelper _browserHelper;

    public RepositoryProvider(Repository repository, IBrowserHelper browserHelper)
    {
        _repository = repository;
        _browserHelper = browserHelper;
    }

    public List<SearchResult> GetSearchResults(string searchString)
    {
        var searchResults = new List<SearchResult>
        {
            new LinkSearchResult
            {
                Title = "Commits",
                Description = "",
                Icon = "",
                BrowserHelper = _browserHelper,
                Link = _repository.HtmlUrl + "/commits"
            },
            new LinkSearchResult
            {
                Title = "Issues",
                Description = "",
                Icon = "",
                BrowserHelper = _browserHelper,
                Link = _repository.HtmlUrl + "/issues"
            },
            new LinkSearchResult
            {
                Title = "Pull Request",
                Description = "",
                Icon = "",
                BrowserHelper = _browserHelper,
                Link = _repository.HtmlUrl + "/pulls"
            }
        };

        return searchResults;
    }
}

public class LinkSearchResult : SearchResult
{
    public required string Link { get; set; }
    public required IBrowserHelper BrowserHelper { get; set; }

    public override void Execute(IExecutionContext executionContext)
    {
        BrowserHelper.OpenWebsite(Link);
    }
}
