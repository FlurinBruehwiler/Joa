using JoaLauncher.Api.Providers;

namespace Joa.Steps;

public class ProviderWrapper
{
    public required IProvider Provider { get; set; }
    public Func<string, bool>? Condition { get; set; }
}