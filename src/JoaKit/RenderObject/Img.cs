using System.ComponentModel;
using SkiaSharp;
using Svg.Skia;

namespace JoaKit.RenderObject;

public class Img : RenderObject
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string PSrc { get; set; }

    public Img(string src)
    {
        PSrc = src;

        if (!s_svgCache.TryGetValue(PSrc, out var svg))
        {
            svg = new SKSvg();
            svg.Load(PSrc);
            s_svgCache.Add(PSrc, svg);
        }

        if (svg.Picture is null)
            return;
        
        PWidth = new SizeDefinition(svg.Picture.CullRect.Width, SizeKind.Pixel);
        PHeight = new SizeDefinition(svg.Picture.CullRect.Height, SizeKind.Pixel);
    }

    public override void Render(SKCanvas canvas)
    {
        canvas.DrawPicture(s_svgCache[PSrc].Picture, PComputedX, PComputedY);
    }

    private static readonly Dictionary<string, SKSvg> s_svgCache = new();
}