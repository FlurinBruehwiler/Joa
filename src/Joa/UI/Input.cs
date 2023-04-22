using JoaKit;
using Modern.WindowKit.Input;

namespace Joa.UI;

public class Input : IComponent
{
    private string _input = string.Empty;

    [Parameter]
    public Txt Txt { get; set; } = null!;
    
    public RenderObject Build()
    {
        Txt.PText = _input;
        
        return new Div
            {
                Txt
            }
            .OnKeyDown(OnKeyDown)
            .OnTextInput(OnTextInput);
    }
    
    private void OnTextInput(string s, RawInputModifiers modifiers)
    {
        if (modifiers != RawInputModifiers.Control)
        {
            _input += s;
        }
    }

    private void OnKeyDown(Key key, RawInputModifiers modifiers)
    {
        if (key != Key.Back) return;
        
        if (modifiers == RawInputModifiers.Control)
        {
            _input = _input.TrimEnd();

            if (!_input.Contains(' '))
            {
                _input = string.Empty;
            }

            for (var i = _input.Length - 1; i > 0; i--)
            {
                if (_input[i] != ' ') continue;
                _input = _input[..(i + 1)];
                break;
            }
        }
        else
        {
            if (_input.Length != 0)
            {
                _input = _input.Remove(_input.Length - 1);
            }
        }
    }
}