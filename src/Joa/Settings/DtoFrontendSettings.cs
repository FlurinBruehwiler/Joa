namespace Joa.Settings;

public class DtoSetting
{
    public List<DtoPlugin> Plugins { get; set; } = new();
}

public class DtoPlugin
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<DtoField> Fields { get; set; } = new();
    public Dictionary<Guid, DtoTemplate> Templates { get; set; } = new();
}

public class DtoField
{
    public Guid TemplateId { get; set; }
    //Could be either a primitive type or a list of fields
    public object Value { get; set; } 
}

public class DtoTemplate
{
    public string Name { get; set; }
}