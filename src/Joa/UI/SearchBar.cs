using JoaKit;
using Modern.WindowKit.Input;

namespace Joa.UI;

public class SearchBar : IComponent
{
    private string _text = string.Empty;
    private List<string> _searchResults = Enumerable.Range(0, 10).Select(x => x.ToString()).ToList();

    public RenderObject Render()
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
                .OnTextInput(s => { _text += s; })
                .XAlign(XAlign.Center)
                .Padding(10)
                .Gap(10)
                .Dir(Dir.Row),
            new Div()
                .Items(_searchResults.Select(x =>
                    new SearchResultComponent(x).Key(x)
                ))
        };
    }

    private void OnKeyDown(Key key)
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
    }
}