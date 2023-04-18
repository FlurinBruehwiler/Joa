using SkiaSharp;

namespace JoaKit;



public record struct ColorDefinition(float Red, float Gree, float Blue, float Transparency = 255);

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