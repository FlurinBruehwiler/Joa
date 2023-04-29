using System.ComponentModel;
using System.Runtime.CompilerServices;
using SkiaSharp;

namespace JoaKit;

public class Img : RenderObject
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string PSrc { get; }

    public Img(string src, [CallerLineNumber] int lineNumer = -1, [CallerFilePath] string filePath = "")
    {
        PLineNumber = lineNumer;
        PFilePath = filePath;
        PSrc = src;

        if (!ImgCache.TryGetValue(PSrc, out var img))
        {
            img = SKBitmap.Decode(PSrc);
            
            ImgCache.Add(PSrc, img);
        }

        PWidth = new SizeDefinition(img.Width, SizeKind.Pixel);
        PHeight = new SizeDefinition(img.Height, SizeKind.Pixel);
    }

    public override void Render(SKCanvas canvas, RenderContext renderContext)
    {
        canvas.DrawBitmap(ImgCache[PSrc], PComputedX, PComputedY);
    }

    private static readonly Dictionary<string, SKBitmap> ImgCache = new();
}