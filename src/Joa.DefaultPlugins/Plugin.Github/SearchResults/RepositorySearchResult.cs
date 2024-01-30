using System.Diagnostics.CodeAnalysis;
using Github.Providers;
using JoaLauncher.Api;
using Octokit;

namespace Github.SearchResults;

public class RepositorySearchResult : SearchResult
{
    private readonly Repository _repository;

    [SetsRequiredMembers]
    public RepositorySearchResult(Repository repository)
    {
        _repository = repository;
        Title = repository.Name;
        Description = repository.Description;
        Icon = string.Empty;
    }

    public override void Execute(IExecutionContext executionContext)
    {
        executionContext
            .AddStepBuilder()
            .AddProvider<RepositoryProvider, Repository>(_repository)
            .WithOptions(new StepOptions
            {
                ShowSearchResultsAllways = true
            });
    }
}