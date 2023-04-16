using JoaKit;
using JoaKit.RenderObjects;
using JoaKitTypes;
using Modern.WindowKit.Input;

namespace Joa.UI;

public class TestComponent : UiComponent
{
    private string _text = string.Empty;

    [Parameter]
    public string Test { get; set; }
    
    public override RenderObject Render()
    {
        
        
        return new Div
            {
                new Img("./battery.svg"),
                new Txt(_text)
                    .Size(30)
                    .VAlign(TextAlign.Center)
            }.Color(40,40, 40)
            .OnKeyDown(key =>
            {
                if (key == Key.Back)
                {
                    if (_text.Length != 0)
                    {
                        _text = _text.Remove(_text.Length - 1);
                    }    
                }

                if (key == Key.Escape)
                {
                    
                }
            })
            .OnTextInput(s =>
            {
                _text += s;
            })
            .XAlign(XAlign.Center)
            .Padding(10)
            .Gap(10)
            .Dir(Dir.Row);
    }
}