using JoaKit;

namespace Joa.UI.Components;

public class DropDown : Component
{
    public override RenderObject Build()
    {
        return new Div
        {
            new Txt("An Option 2")
                .Size(18)
                .VAlign(TextAlign.Center),
            new Div
                {
                    new JoaKit.Svg("expand.svg")
                }.Width(30)
                .Height(30)
        }.Width(200)
            .PaddingLeft(10)
            .PaddingRight(5)
            .XAlign(XAlign.Center)
            .Dir(Dir.Horizontal)
            .Height(35)
            .Color(40, 40, 40)
            .BorderWidth(1)
            .BorderColor(200, 200, 200, 100)
            .Radius(5);
    }
}