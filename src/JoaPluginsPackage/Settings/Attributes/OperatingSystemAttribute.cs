namespace JoaPluginsPackage.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class OperatingSystemAttribute : Attribute
{
    public OperatingSystem OperatingSystem { get; }

    public OperatingSystemAttribute(OperatingSystem operatingSystem)
    {
        OperatingSystem = operatingSystem;
    }
}