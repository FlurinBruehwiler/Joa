using System.ComponentModel;
using SkiaSharp;

namespace JoaKit.RenderObjects;

public class Txt : RenderObject
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string PText { get; set; } = string.Empty;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public float PSize { get; set; } = 15;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TextAlign PhAlign { get; set; } = TextAlign.Start;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TextAlign PvAlign { get; set; } = TextAlign.Start;
    
    public Txt(string content)
    {
        PText = content;
    }

    public Txt Width(float width, SizeKind sizeKind = SizeKind.Pixel)
    {
        PWidth = new SizeDefinition(width, sizeKind);
        return this;
    }

    public Txt Height(float height, SizeKind sizeKind = SizeKind.Pixel)
    {
        PHeight = new SizeDefinition(height, sizeKind);
        return this;
    }

    public Txt Size(float size)
    {
        PSize = size;
        return this;
    }

    public Txt HAlign(TextAlign textAlign)
    {
        PhAlign = textAlign;
        return this;
    }

    public Txt VAlign(TextAlign textAlign)
    {
        PvAlign = textAlign;
        return this;
    }

    public override void Render(SKCanvas canvas)
    {
        var paint = new SKPaint
        {
            Color = new SKColor(255, 255, 255),
            IsAntialias = true,
            TextSize = PSize,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright)
        };

        var path = paint.GetTextPath(PText, PComputedX, PComputedY);
        path.GetBounds(out var rect);

        var actualX = PComputedX;
        var actualY = PComputedY;

        actualY += PvAlign switch
        {
            TextAlign.Start => PSize,
            TextAlign.End => PComputedHeight,
            TextAlign.Center => PComputedHeight / 2 + PSize * 0.74f / 2, // you ask "Why *0.74?" -> Because. 
            _ => throw new ArgumentOutOfRangeException()
        };

        actualX += PhAlign switch
        {
            TextAlign.End => PComputedWidth - rect.Width,
            TextAlign.Center => PComputedWidth / 2 - rect.Width / 2,
            TextAlign.Start => 0,
            _ => throw new ArgumentOutOfRangeException()
        };

        canvas.DrawText(PText, actualX, actualY, paint);
    }
}

public enum TextAlign
{
    Start,
    Center,
    End
}