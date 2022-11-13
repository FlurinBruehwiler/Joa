using OperatingSystem = Joa.Api.Enums.OperatingSystem;

namespace Joa.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class OperatingSystemAttribute : Attribute
{
    public Enums.OperatingSystem OperatingSystem { get; }

    public OperatingSystemAttribute(Enums.OperatingSystem operatingSystem)
    {
        OperatingSystem = operatingSystem;
    }

    public OperatingSystemAttribute()
    {
        
    }
}