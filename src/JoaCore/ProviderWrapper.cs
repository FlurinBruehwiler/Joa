using JoaPluginsPackage.Providers;

namespace JoaCore;

public class ProviderWrapper
{
    public required IProvider Provider { get; set; }
    public Delegate? Condition { get; set; }
}