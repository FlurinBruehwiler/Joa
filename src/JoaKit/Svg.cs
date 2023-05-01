using System.ComponentModel;
using System.Runtime.CompilerServices;
using Modern.WindowKit;
using SkiaSharp;
using Svg.Skia;

namespace JoaKit;

public class Svg : RenderObject
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string PSrc { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public float PScaleX { get; private set; } = 1;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public float PScaleY { get; private set; } = 1;
    
    public Svg(string src, [CallerLineNumber] int lineNumer = -1, [CallerFilePath] string filePath = "")
    {
        PLineNumber = lineNumer;
        PFilePath = filePath;
        PSrc = src;

        if (!SSvgCache.TryGetValue(PSrc, out var svg))
        {
            svg = new SKSvg();
            svg.Load(PSrc);
            SSvgCache.Add(PSrc, svg);
        }

        if (svg.Picture is null)
            return;

        PWidth = new SizeDefinition(svg.Picture.CullRect.Width, SizeKind.Pixel);
        PHeight = new SizeDefinition(svg.Picture.CullRect.Height, SizeKind.Pixel);
    }

    public override void Render(SKCanvas canvas, RenderContext renderContext)
    {
        var matrix = SKMatrix.CreateScale(PScaleX, PScaleY);
        matrix.TransX = PComputedX;
        matrix.TransY = PComputedY;
        canvas.DrawPicture(SSvgCache[PSrc].Picture, ref matrix);
    }

    public Svg Scale(float scale)
    {
        PScaleX = scale;
        PScaleY = scale;
        return this;
    }

    public Svg Scale(float x, float y)
    {
        PScaleX = x;
        PScaleY = y;
        return this;
    }

    private static readonly Dictionary<string, SKSvg> SSvgCache = new();
}