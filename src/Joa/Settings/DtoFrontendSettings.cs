using System.Reflection;

namespace Joa.Settings;

public class DtoPropertyDescription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public List<CustomAttributeData> Attributes { get; set; } = new();
}

public class DtoPropertyInstance
{
    public object Value { get; set; }
    public Guid SettingsDescriptionId { get; set; }
}

public class DtoFronten1dSettings
{
    public List<DtoPropertyDescription> SettingsDescriptions { get; set; } = new();
    public List<DtoPropertyInstance> Properties { get; set; } = new();
}