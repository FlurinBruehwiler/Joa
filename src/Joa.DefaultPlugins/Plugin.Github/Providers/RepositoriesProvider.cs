using JoaLauncher.Api;
using JoaLauncher.Api.Providers;

namespace Github.Providers;

public class RepositoriesProvider : IProvider
{
    private readonly GithubCache _githubCache;

    public RepositoriesProvider(GithubCache githubCache)
    {
        _githubCache = githubCache;
    }

    public List<SearchResult> GetSearchResults(string searchString)
    {
        return _githubCache.Repositories;
    }
}