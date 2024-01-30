using Github.SearchResults;
using JoaLauncher.Api;
using JoaLauncher.Api.Plugin;
using Octokit;

namespace Github;


public class GithubCache : IAsyncCache
{
    private readonly GitHubClient _github;

    public GithubCache()
    {
        _github = new GitHubClient(new ProductHeaderValue("JoaLauncherGithubPlugin"));
    }
    public List<SearchResult> Repositories { get; set; } = null!;

    public async Task UpdateIndexesAsync()
    {
        var repositories = await _github.Repository.GetAllForUser("FlurinBruehwiler");
        Repositories = repositories.Select(x => new RepositorySearchResult(x)).Cast<SearchResult>().ToList();
    }
}