using JoaLauncher.Api.Providers;

namespace JoaInterface.Step;

public class ProviderWrapper
{
    public required IProvider Provider { get; set; }
    public Func<string, bool>? Condition { get; set; }
}