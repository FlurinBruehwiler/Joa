using JoaKit;

namespace Joa.UI.Components;

public class TextBox : Component
{
    [Extension] public string Value { get; set; } = string.Empty;

    [Extension] public Action<string>? OnChange { get; set; }

    [Extension] public Func<string, Task>? OnChangeAsync { get; set; }

    public override RenderObject Build()
    {
        return
            new Div
                {
                    new InputComponent()
                        .Value(Value)
                        .OnChange(s =>
                        {
                            Value = s;
                            OnChange?.Invoke(s);
                        })
                        .OnChangeAsync(async s =>
                        {
                            Value = s;
                            if (OnChangeAsync is not null)
                            {
                                await OnChangeAsync(s);
                            }
                        })
                        .Size(20)
                }.Height(35)
                .Width(200)
                .Color(40, 40, 40)
                .BorderWidth(1)
                .Padding(10)
                .BorderColor(200, 200, 200, 100)
                .Radius(5);
    }
}