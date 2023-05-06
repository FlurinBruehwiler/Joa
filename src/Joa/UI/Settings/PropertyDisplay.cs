using Joa.PluginCore;
using Joa.Settings;
using Joa.UI.Components;
using JoaKit;

namespace Joa.UI.Settings;

public class PropertyDisplay : Component
{
    private readonly SettingsManager _settingsManager;

    [Parameter]
    public PluginDefinition PluginDefinition { get; set; } = null!;

    [Parameter]
    public PropertyInstance Property { get; set; } = null!;

    public PropertyDisplay(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public override RenderObject Build()
    {
        var type = Property.PropertyDescription.PropertyInfo.PropertyType;

        if (type == typeof(int) || type == typeof(float) || type == typeof(string))
        {
            return new Div
                {
                    new Txt(Property.PropertyDescription.PropertyInfo.Name)
                        .VAlign(TextAlign.Center)
                        .Size(17),
                    new TextBoxComponent()
                }
                .Height(50)
                .Color(36, 36, 36)
                .XAlign(XAlign.Center)
                .Dir(Dir.Horizontal)
                .Padding(8)
                .MAlign(MAlign.SpaceBetween)
                .Radius(7);
        }

        if (type == typeof(bool))
        {
            return new Div
            {
                new Txt(Property.PropertyDescription.PropertyInfo.Name)
                    .VAlign(TextAlign.Center)
                    .Size(17),
                new CheckboxComponent()
            }.Height(50)
                .Color(36, 36, 36)
                .Padding(8)
                .Radius(7)
                .XAlign(XAlign.Center)
                .Dir(Dir.Horizontal);
        }

        return new Div().Height(0);
    }

    private async Task PropertyChanged(string arg)
    {
        Property.SetValue(arg);
        await _settingsManager.SaveSettingsToJsonAsync();
    }
}