using JoaKit.RenderObject;
using SkiaSharp;

namespace JoaKit;

public enum SizeKind
{
    Percentage,
    Pixel
}

public record struct SizeDefinition(float Value, SizeKind Kind);

public record struct ColorDefinition(float Red, float Gree, float Blue, float Transparency);

public static class StaticStuff
{
    public static SKPaint Black = new()
    {
        Color = new SKColor(),
        IsAntialias = true
    };
}

public class TextDefinition
{
    public SizeDefinition Width { get; set; } = new(100, SizeKind.Percentage);
    public SizeDefinition Height { get; set; } = new(100, SizeKind.Percentage);
}

public interface IDefinition
{
}

public abstract class UiComponent
{
    public abstract Div Render();
}