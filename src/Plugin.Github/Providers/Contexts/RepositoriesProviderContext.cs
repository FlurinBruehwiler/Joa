using JoaPluginsPackage;

namespace Github.Providers.Contexts;

public class RepositoriesProviderContext : ISearchProviderContext
{
    public string? SearchString { get; set; }
}