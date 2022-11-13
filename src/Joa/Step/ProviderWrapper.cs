using JoaPluginsPackage.Providers;

namespace JoaInterface.Step;

public class ProviderWrapper
{
    public required IProvider Provider { get; set; }
    public Func<string, bool>? Condition { get; set; }
}