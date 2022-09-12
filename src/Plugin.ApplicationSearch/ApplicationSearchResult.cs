using System.Diagnostics;
using JoaPluginsPackage;
using JoaPluginsPackage.Injectables;

namespace ApplicationSearch;

public class ApplicationSearchResult : ISearchResult
{
    public string FilePath { get; init; } = default!;
    public string Caption { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
    public List<ContextAction>? Actions { get; init; }
    public void Execute()
    {
        var info = new ProcessStartInfo ( FilePath )
        {
            UseShellExecute = true
        };
        Process.Start(info);
    }
}