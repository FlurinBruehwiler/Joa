using JoaKit;

namespace Joa.UI.Components;

public class Checkbox : Component
{
    [Extension]
    public bool IsChecked { get; set; }

    public override RenderObject Build()
    {
        return new Div
            {
                IsChecked
                    ? new JoaKit.Svg("check.svg")
                    : new Empty()
            }
            .Width(20)
            .BorderColor(200, 200, 200, 100)
            .Color(40, 40, 40)
            .BorderWidth(1)
            .Radius(4)
            .Height(20)
            .OnClick(() => IsChecked = !IsChecked);
    }
}