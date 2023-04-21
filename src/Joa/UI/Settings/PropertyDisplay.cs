using System.Diagnostics.CodeAnalysis;
using Joa.PluginCore;
using Joa.Settings;
using JoaKit;

namespace Joa.UI.Settings;

public class PropertyDisplay : IComponent
{
    [Parameter]
    public PluginDefinition PluginDefinition { get; set; } = null!;
    
    [Parameter]
    public PropertyInstance Property { get; set; } = null!;

    
    public RenderObject Build()
    {
        var type = Property.PropertyDescription.PropertyInfo.PropertyType;

        if (type == typeof(int) || type == typeof(float) || type == typeof(string))
        {
            return new Div
                {
                    new Txt(Property.PropertyDescription.PropertyInfo.Name)
                    //Text Input
                }
                .Height(50)
                .Color(36, 36, 36)
                .XAlign(XAlign.Center)
                .Padding(8)
                .MAlign(MAlign.SpaceBetween)
                .Radius(4);
        }

        if (type == typeof(bool))
        {
            return new Div
            {
                new Txt(Property.PropertyDescription.PropertyInfo.Name)
                //Checkbox
            };
        }

        return new Div();
    }
}