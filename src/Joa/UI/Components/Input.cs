using JoaKit;
using Modern.WindowKit.Input;

namespace Joa.UI.Components;

public class Input : Component
{
    [Extension]
    public string Value { get; set; } = string.Empty;

    [Extension]
    public Action<Key, RawInputModifiers>? OnKeyDown { get; set; }

    [Extension]
    public Func<Key, RawInputModifiers, Task>? OnKeyDownAsync { get; set; }

    [Extension]
    public Action<string>? OnChange { get; set; }

    [Extension]
    public Func<string, Task>? OnChangeAsync { get; set; }

    [Extension]
    public bool AutoFocus { get; set; } = false;

    [Extension]
    public float Size { get; set; } = 30;

    public override RenderObject Build()
    {
        return new Div
            {
                new Txt(Value)
                    .Size(Size)
                    .VAlign(TextAlign.Center)
            }
            .OnKeyDown(OnKeyDownInternal)
            .OnTextInput(OnTextInput)
            .AutoFocus(AutoFocus);
    }

    private void OnTextInput(string s, RawInputModifiers modifiers)
    {
        if (modifiers != RawInputModifiers.Control)
        {
            Value += s;
            ContentChanged();
        }
    }

    private void ContentChanged()
    {
        OnChange?.Invoke(Value);
    }

    private async Task OnKeyDownInternal(Key key, RawInputModifiers modifiers)
    {
        var initialValue = Value;

        if (key == Key.Back)
        {
            if (modifiers == RawInputModifiers.Control)
            {
                Value = Value.TrimEnd();

                if (!Value.Contains(' '))
                {
                    Value = string.Empty;
                }

                for (var i = Value.Length - 1; i > 0; i--)
                {
                    if (Value[i] != ' ') continue;
                    Value = Value[..(i + 1)];
                    break;
                }
            }
            else
            {
                if (Value.Length != 0)
                {
                    Value = Value.Remove(Value.Length - 1);
                }
            }
        }

        if (OnKeyDownAsync is not null)
        {
            await OnKeyDownAsync(key, modifiers);
        }

        if (initialValue != Value)
        {
            ContentChanged();
        }

        OnKeyDown?.Invoke(key, modifiers);
    }
}