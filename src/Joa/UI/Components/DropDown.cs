using JoaKit;

namespace Joa.UI.Components;

public class DropDown : Component
{
    private bool _isExpanded;
    private List<string> _options;
    private string _selectedOption;

    public DropDown()
    {
        _options = Enumerable.Range(0, 5).Select(x => $"Option {x}").ToList();
        _selectedOption = _options.FirstOrDefault() ?? string.Empty;
    }

    public override RenderObject Build()
    {
        return new Div
            {
                new Div
                    {
                        new Txt(_selectedOption)
                            .Size(18)
                            .VAlign(TextAlign.Center),
                        new Div
                            {
                                new JoaKit.Svg("expand.svg")
                            }.Width(35)
                            .Height(35)
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
                        .Absolute(top: 40)
                        .Items(_options.Select(x =>
                            new Div
                                {
                                    new Txt(x)
                                        .VAlign(TextAlign.Center)
                                        .Size(18)
                                }
                                .PaddingLeft(10)
                                .Radius(5)
                                .HoverColor(50, 50, 50)
                                .OnClick(() => _selectedOption = x)
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