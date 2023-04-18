using Joa.PluginCore;
using Joa.Steps;
using JoaKit;
using JoaLauncher.Api;
using Microsoft.Extensions.DependencyInjection;
using Modern.WindowKit;
using Modern.WindowKit.Input;
using Modern.WindowKit.Platform;
using Key = Modern.WindowKit.Input.Key;

namespace Joa.UI;

public class SearchBar : IComponent
{
    private readonly IWindowImpl _window;
    private readonly JoaManager _joaManager;
    private string _input = string.Empty;
    private List<PluginSearchResult> _searchResults = new();
    private Stack<Step> _steps = new();
    private readonly Search _search;

    public SearchBar(IWindowImpl window, JoaManager joaManager)
    {
        joaManager.NewScope();
        _window = window;
        _joaManager = joaManager;
        _search = _joaManager.CurrentScope!.ServiceProvider.GetRequiredService<Search>();

        _steps.Push(new Step
        {
            Providers = joaManager.CurrentScope!.ServiceProvider.GetRequiredService<PluginManager>().GlobalProviders,
            Name = "Global Step",
            Options = new StepOptions()
        });
        
        
        _window.Resize(new Size(_window.ClientSize.Width, _window.ClientSize.Height + 8 * 60));
    }

    public RenderObject Build()
    {
        return new Div
        {
            new Div
                {
                    new Img("./battery.svg"),
                    new Txt(_input)
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
            new Div()
                .Items(_searchResults.Select(x =>
                    new Div
                        {
                            new Txt(x.SearchResult.Title)
                        }.Color(40, 40, 40)
                        .BorderWidth(2)
                ))
        };
    }

    private void TextChanged()
    {
        if (_input == string.Empty)
            return;

        _searchResults = _search.UpdateSearchResults(_steps.Peek(), _input);

        SearchResultsHaveChanged();
    }

    private void SearchResultsHaveChanged()
    {
    }

    private void OnTextInput(string s, RawInputModifiers modifiers)
    {
        if (modifiers != RawInputModifiers.Control)
        {
            _input += s;
            TextChanged();
        }
    }

    private void OnKeyDown(Key key, RawInputModifiers modifiers)
    {
        if (key == Key.Back)
        {
            if (modifiers == RawInputModifiers.Control)
            {
                _input = _input.TrimEnd();

                if (!_input.Contains(' '))
                {
                    _input = string.Empty;
                }

                for (var i = _input.Length - 1; i > 0; i--)
                {
                    if (_input[i] == ' ')
                    {
                        _input = _input[..(i + 1)];
                        break;
                    }
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

        if (key == Key.Escape)
        {
            // _window.Hide();
        }
        TextChanged();
    }
}