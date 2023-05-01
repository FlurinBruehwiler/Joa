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
    public bool PFill { get; set; }

    public Svg(string src, [CallerLineNumber] int lineNumer = -1, [CallerFilePath] string filePath = "")
    {
        src = "Assets/" + src;
        
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

        var svgSize = SSvgCache[PSrc].Picture.CullRect;
        
        PWidth = new SizeDefinition(svgSize.Width, SizeKind.Pixel);
        PHeight = new SizeDefinition(svgSize.Height, SizeKind.Pixel);
    }

    public override void Render(SKCanvas canvas, RenderContext renderContext)
    {
        var svgSize = SSvgCache[PSrc].Picture.CullRect;

        var matrix = SKMatrix.Identity;

        if (PFill)
        {
            var availableRatio = PComputedWidth / PComputedHeight;
            var currentRatio = svgSize.Width / svgSize.Height;

            float factor;
            
            if (availableRatio > currentRatio) //Height is the limiting factor
            {
                factor = PComputedHeight / svgSize.Height;
            }
            else //Width is the limiting factor
            {
                factor = PComputedWidth / svgSize.Width;
            }

            matrix.ScaleX = factor;
            matrix.ScaleY = factor;
        }
        
        matrix.TransX = PComputedX;
        matrix.TransY = PComputedY;
        
        canvas.DrawPicture(SSvgCache[PSrc].Picture, ref matrix);
    }

    public Svg Fill(bool fill = true)
    {
        PWidth = new SizeDefinition(100, SizeKind.Percentage);
        PHeight = new SizeDefinition(100, SizeKind.Percentage);
        PFill = fill;
        return this;
    }

    private static readonly Dictionary<string, SKSvg> SSvgCache = new();
}