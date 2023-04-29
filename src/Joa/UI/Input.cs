using JoaKit;
using Modern.WindowKit.Input;

namespace Joa.UI;

public class Input : IComponent
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

    public RenderObject Build()
    {
        return new Div
            {
                new Txt(Value)
                    .Size(30)
                    .VAlign(TextAlign.Center)
            }
            .OnKeyDown(OnKeyDownInternal)
            .OnTextInput(OnTextInput)
            .AutoFocus();
    }

    private void OnTextInput(string s, RawInputModifiers modifiers)
    {
        if (modifiers != RawInputModifiers.Control)
        {
            Value += s;
        }

        ContentChanged();
    }

    private void ContentChanged()
    {
        OnChange?.Invoke(Value);
    }

    private async Task OnKeyDownInternal(Key key, RawInputModifiers modifiers)
    {
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

        ContentChanged();
        OnKeyDown?.Invoke(key, modifiers);
    }
}