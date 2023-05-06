using System.Runtime.InteropServices;
using Joa.Hotkey;
using Joa.PluginCore;
using Joa.Steps;
using Joa.UI.Components;
using JoaKit;
using JoaLauncher.Api;
using Modern.WindowKit;
using Modern.WindowKit.Input;
using Modern.WindowKit.Platform;
using Key = Modern.WindowKit.Input.Key;

namespace Joa.UI;

public class SearchBar : Component
{
    private readonly IWindowImpl _window;
    private string _input = string.Empty;
    private List<PluginSearchResult> _searchResults = new();
    private int _selectedResult;
    private Stack<Step> _steps = new();
    private readonly Search _search;
    public const int SearchBoxHeight = 60;
    private const int StepsHeight = 30;
    private const int SearchResultHeight = 60;
    public const int Width = 600;

    public SearchBar(IWindowImpl window, GlobalHotKey globalHotKey, Search search, PluginManager pluginManager)
    {
        _window = window;
        _search = search;

        window.LostFocus = HideWindow;

        SearchResultsHaveChanged();

        _steps.Push(new Step
        {
            Providers = pluginManager.GlobalProviders,
            Name = "Global Step",
            Options = new StepOptions()
        });

        globalHotKey.InitialHotKeyRegistration(() =>
        {
            _window.Show(true, false);

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
            External.SetFocus(_window.Handle.Handle);
            External.SetForegroundWindow(_window.Handle.Handle);
        });
    }

    public override RenderObject Build()
    {
        return new Div
        {
            new Div
                {
                    new Div
                    {
                        new JoaKit.Svg("./battery.svg")
                    }.Width(40),
                    new InputComponent()
                        .OnKeyDownAsync(OnKeyDownAsync)
                        .Value(_input)
                        .AutoFocus(true)
                        .OnChange(s =>
                        {
                            _input = s;
                            TextChanged();
                        })
                }.Color(40, 40, 40)
                .XAlign(XAlign.Center)
                .Padding(10)
                .Gap(10)
                .Height(SearchBoxHeight)
                .Dir(Dir.Horizontal),
            new Div()
                .Items(_steps.Reverse().Select(x =>
                    new Div
                        {
                            new Txt(x.Name).VAlign(TextAlign.Center).HAlign(TextAlign.Center)
                        }.Width(100)
                        .MAlign(MAlign.Center)
                        .Color(60, 60, 60)
                        .Radius(5)
                )).Dir(Dir.Horizontal)
                .Padding(4)
                .Gap(8)
                .Height(StepsHeight)
                .Color(40, 40, 40),
            new Div()
                .Items(_searchResults.Select((x, i) =>
                    new SearchResultComponent(x, _selectedResult == i)
                        .Key(x.SearchResult.Title)
                ))
        };
    }

    private async Task OnKeyDownAsync(Key key, RawInputModifiers modifiers)
    {
        if (key == Key.Escape)
        {
            HideWindow();
        }
        if (key == Key.Down)
        {
            if (_selectedResult < _searchResults.Count - 1)
            {
                _selectedResult++;
            }
        }
        if (key == Key.Up)
        {
            if (_selectedResult > 0)
            {
                _selectedResult--;
            }
        }
        if (key == Key.Enter)
        {
            if (_searchResults.Count != 0)
            {
                // _searchResults[_selectedResult].SearchResult.Actions!.First(action => action.Id == key.ToString());
                var newStep = await _search.ExecuteCommand(_searchResults[_selectedResult].SearchResult, _searchResults[_selectedResult].SearchResult.Actions!.First());
                if (newStep is not null)
                {
                    _steps.Push(newStep);
                }
                else
                {
                    HideWindow();
                }
                _input = string.Empty;
                _searchResults.Clear();
                SearchResultsHaveChanged();
            }
        }
    }

    private void TextChanged()
    {
        if (_input == string.Empty && _steps.Count == 1)
        {
            _searchResults.Clear();
        }
        else
        {
            _searchResults = _search.UpdateSearchResults(_steps.Peek(), _input);
        }

        SearchResultsHaveChanged();
    }

    private void SearchResultsHaveChanged()
    {
        _selectedResult = 0;
        _window.Resize(new Size(_window.ClientSize.Width,
            SearchBoxHeight + StepsHeight + _searchResults.Count * SearchResultHeight));
    }

    private void HideWindow()
    {
        _window.Hide();
        ClearSteps();
        _input = string.Empty;
        _searchResults.Clear();
        SearchResultsHaveChanged();
        StateHasChanged();
    }

    private void ClearSteps()
    {
        while (_steps.Count > 1)
        {
            _steps.Pop();
        }
    }
}