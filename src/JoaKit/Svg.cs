using System.ComponentModel;
using System.Runtime.CompilerServices;
using SkiaSharp;
using Svg.Skia;

namespace JoaKit;

public class Svg : RenderObject
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string PSrc { get; }

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
        canvas.DrawPicture(SSvgCache[PSrc].Picture, PComputedX, PComputedY);
    }

    private static readonly Dictionary<string, SKSvg> SSvgCache = new();
}