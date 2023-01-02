namespace Joa.Settings;

public class DtoSetting
{
    public List<DtoPlugin> Plugins { get; set; } = new();
}

public class DtoPlugin
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public List<DtoField> Fields { get; set; } = new();
    public Dictionary<Guid, DtoTemplate> Templates { get; set; } = new();
}

public class DtoField
{
    public required Guid TemplateId { get; set; }
    //Could be either a primitive type or a list of fields
    public required object Value { get; set; } 
}

public class DtoTemplate
{
    public required string Name { get; set; }
}