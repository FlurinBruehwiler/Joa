using Joa.Hotkey;
using JoaKit;
using Modern.WindowKit;
using Modern.WindowKit.Input;
using Modern.WindowKit.Platform;
using Key = Modern.WindowKit.Input.Key;

namespace Joa.UI;

public class SearchBar : IComponent
{
    private readonly IWindowImpl _window;
    private string _text = string.Empty;
    private readonly List<string> _searchResults = Enumerable.Range(0, 5).Select(x => x.ToString()).ToList();

    public SearchBar(IWindowImpl window)
    {
        _window = window;
        // window.Resize(new Size(window.ClientSize.Width, window.ClientSize.Height + _searchResults.Count * 60));
    }
    
    public RenderObject Build()
    {
        return new Div
        {
            new Div
                {
                    new Img("./battery.svg"),
                    new Txt(_text)
                        .Size(30)
                        .VAlign(TextAlign.Center)
                }.Color(40, 40, 40)
                .OnKeyDown(OnKeyDown)
                .OnTextInput(OnTextInput)
                .XAlign(XAlign.Center)
                .Padding(10)
                .Gap(10)
                .Height(60)
                .Dir(Dir.Row),
            // new Div()
            //     .Items(_searchResults.Select(x =>
            //         new SearchResultComponent(x).Key(x)
            //     ))
        };
    }

    private void OnTextInput(string s, RawInputModifiers modifiers)
    {
        if (modifiers == RawInputModifiers.None)
        {
            _text += s;
        }
    }
    
    private void OnKeyDown(Key key, RawInputModifiers modifiers)
    {
        if (key == Key.Back)
        {
            if (modifiers == RawInputModifiers.Control)
            {
                _text = _text.TrimEnd();
                
                if (!_text.Contains(' '))
                {
                    _text = string.Empty;
                }

                for (var i = _text.Length - 1; i > 0; i--)
                {
                    if (_text[i] == ' ')
                    {
                        _text = _text[..(i + 1)];
                        break;
                    }
                }
            }
            else
            {
                if (_text.Length != 0)
                {
                    _text = _text.Remove(_text.Length - 1);
                }
            }
        }

        if (key == Key.Escape)
        {
            // _window.Hide();
        }
    }
}