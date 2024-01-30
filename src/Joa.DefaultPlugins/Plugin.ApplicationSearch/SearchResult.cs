using System.Diagnostics;
using JoaLauncher.Api;

namespace ApplicationSearch;

public class ApplicationSearchResult : SearchResult
{
    public string FilePath { get; init; } = default!;

    public override void Execute(IExecutionContext executionContext)
    {
        Process.Start(new ProcessStartInfo
        { Arguments = $"""/C "{FilePath}" """, FileName = "cmd", WindowStyle = ProcessWindowStyle.Hidden, UseShellExecute = true });
    }
}