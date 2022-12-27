using System.Reflection;

namespace Joa.Settings;

public class DtoSettingsDescription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public List<CustomAttributeData> Attributes { get; set; } = new();
}

public class DtoProperty
{
    public object Value { get; set; }
    public Guid SettingsDescriptionId { get; set; }
}

public class DtoFrontendSettings
{
    public List<DtoSettingsDescription> SettingsDescriptions { get; set; } = new();
    public List<DtoProperty> Properties { get; set; } = new();
}