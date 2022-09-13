using OperatingSystem = JoaPluginsPackage.Enums.OperatingSystem;

namespace JoaPluginsPackage.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class OperatingSystemAttribute : Attribute
{
    public OperatingSystem OperatingSystem { get; }

    public OperatingSystemAttribute(OperatingSystem operatingSystem)
    {
        OperatingSystem = operatingSystem;
    }

    public OperatingSystemAttribute()
    {
        
    }
}