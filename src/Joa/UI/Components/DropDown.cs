using JoaKit;

namespace Joa.UI.Components;

public class DropDown : Component
{
    private bool _isExpanded;

    public override RenderObject Build()
    {
        return new Div
            {
                new Div
                    {
                        new Txt("An Option 2")
                            .Size(18)
                            .VAlign(TextAlign.Center),
                        new Div
                            {
                                new JoaKit.Svg("expand.svg")
                            }.Width(30)
                            .Height(30)
                            .OnActive(() => _isExpanded = true)
                            .OnInactive(() => _isExpanded = false)
                    }.Height(35)
                    .PaddingLeft(10)
                    .PaddingRight(5)
                    .Dir(Dir.Horizontal)
                    .OnActive(() => _isExpanded = true)
                    .OnInactive(() => _isExpanded = false),
                _isExpanded
                    ? new Div()
                        .Height(5 * 35)
                        .BorderWidth(1)
                        .BorderColor(200, 200, 200, 100)
                        .Radius(5)
                        .Color(40, 40, 40)
                        .Items(Enumerable.Range(0, 5).Select(x =>
                            new Div
                                {
                                    new Txt($"Option {x}")
                                        .VAlign(TextAlign.Center)
                                        .Size(18)
                                }
                                .PaddingLeft(10)
                                .HoverColor(50, 50, 50)
                                .Key(x.ToString())))
                    : new Empty()
            }
            .Width(200)
            .XAlign(XAlign.Center)
            .Height(35)
            .Color(40, 40, 40)
            .BorderWidth(1)
            .BorderColor(200, 200, 200, 100)
            .Radius(5);
    }
}