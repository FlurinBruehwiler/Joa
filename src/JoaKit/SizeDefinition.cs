using Modern.WindowKit.Platform;

namespace JoaKit;

public struct SizeDefinition
{
    public SizeDefinition(float value, SizeKind kind)
    {
        Value = value;
        Kind = kind;
    }

    public float GetDpiAwareValue(IWindowImpl window)
    {
        if (Kind == SizeKind.Percentage)
            return Value;

        return window.Scale(Value);
    }

    public float Value { get; set; }
    public SizeKind Kind { get; set; }
}