namespace Interfaces.Settings;

[AttributeUsage(AttributeTargets.Property)]
public class SettingPropertyAttribute : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
    public SettingType SettingType { get; set; }
    public string AddButtonText { get; set; }

    public SettingPropertyAttribute()
    {
        Name = "";
        Description = "";
        SettingType = SettingType.Text;
        AddButtonText = "Add";
    }
}