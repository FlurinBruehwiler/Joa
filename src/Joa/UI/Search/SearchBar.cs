global using static Flamui.Ui;
global using static Flamui.Components.Ui;
using System.Runtime.InteropServices;
using Flamui;
using Joa.Hotkey;
using Joa.PluginCore;
using Joa.Steps;
using JoaLauncher.Api;
using Flamui.UiElements;
using SDL2;



namespace Joa.UI.Search;

public class SearchBar : FlamuiComponent
{
    private string _input = string.Empty;
    private List<PluginSearchResult> _searchResults = new();
    private int _selectedResult;
    private Stack<Step> _steps = new();
    private readonly Joa.Search _search;
    public const int SearchBoxHeight = 60;
    private const int StepsHeight = 30;
    private const int SearchResultHeight = 60;
    public const int Width = 600;

    public SearchBar(GlobalHotKey globalHotKey, Joa.Search search, PluginManager pluginManager)
    {
        _search = search;

        // window.LostFocus = HideWindow;

        SearchResultsHaveChanged();

        _steps.Push(new Step
        {
            Providers = pluginManager.GlobalProviders,
            Name = "Global Step",
            Options = new StepOptions()
        });

        globalHotKey.InitialHotKeyRegistration(() =>
        {
            // _window.Show(true, false);

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
            // External.SetFocus(_window.Handle.Handle);
            // External.SetForegroundWindow(_window.Handle.Handle);
        });
    }

    public override void Build()
    {
        DivStart();

            DivStart().Color(40, 40, 40).XAlign(XAlign.Center).Padding(10).Gap(10).Height(SearchBoxHeight).Dir(Dir.Horizontal);

                DivStart();
                    SvgImage("./battery.svg");
                    Input(ref _input, hasFocus:true);
                DivEnd();

            DivEnd();

            DivStart().Dir(Dir.Horizontal).Padding(4).Gap(8).Height(StepsHeight).Color(40, 40, 40);

                foreach (var step in _steps.Reverse())
                {
                    DivStart();

                        Text(step.Name).VAlign(TextAlign.Center).HAlign(TextAlign.Center);

                    DivEnd();
                }

            DivEnd();

            DivStart();

                for (var i = 0; i < _searchResults.Count; i++)
                {
                    var searchResult = _searchResults[i];
                    //searchresultkey
                }

            DivEnd();

        DivEnd();
    }

    private async Task OnKeyDownAsync(SDL.SDL_Scancode key)
    {
        if (key == SDL.SDL_Scancode.SDL_SCANCODE_ESCAPE)
        {
            HideWindow();
        }
        if (key == SDL.SDL_Scancode.SDL_SCANCODE_DOWN)
        {
            if (_selectedResult < _searchResults.Count - 1)
            {
                _selectedResult++;
            }
        }
        if (key == SDL.SDL_Scancode.SDL_SCANCODE_UP)
        {
            if (_selectedResult > 0)
            {
                _selectedResult--;
            }
        }
        if (key == SDL.SDL_Scancode.SDL_SCANCODE_RETURN)
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
        // _window.Resize(new Size(_window.ClientSize.Width,
        //     SearchBoxHeight + StepsHeight + _searchResults.Count * SearchResultHeight));
    }

    private void HideWindow()
    {
        // _window.Hide();
        ClearSteps();
        _input = string.Empty;
        _searchResults.Clear();
        SearchResultsHaveChanged();
        // StateHasChanged();
    }

    private void ClearSteps()
    {
        while (_steps.Count > 1)
        {
            _steps.Pop();
        }
    }
}
