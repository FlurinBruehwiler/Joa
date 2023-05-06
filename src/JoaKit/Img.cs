using System.ComponentModel;
using System.Runtime.CompilerServices;
using SkiaSharp;

namespace JoaKit;

public class Img : RenderObject
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string PSrc { get; }

    private bool _isValid = false;

    public Img(string src, [CallerLineNumber] int lineNumer = -1, [CallerFilePath] string filePath = "")
    {
        PLineNumber = lineNumer;
        PFilePath = filePath;
        PSrc = src;

        if (string.IsNullOrWhiteSpace(src))
            return;

        if (!ImgCache.TryGetValue(PSrc, out var img))
        {
            img = SKBitmap.Decode(PSrc);

            ImgCache.Add(PSrc, img);
        }

        if (img is null)
            return;

        PWidth = new SizeDefinition(img.Width, SizeKind.Pixel);
        PHeight = new SizeDefinition(img.Height, SizeKind.Pixel);
        _isValid = true;
    }

    public override void Render(SKCanvas canvas, RenderContext renderContext)
    {
        if (!_isValid)
            return;

        canvas.DrawBitmap(ImgCache[PSrc], new SKRect(PComputedX, PComputedY, PComputedX + PComputedWidth, PComputedY + PComputedHeight));
    }

    private static readonly Dictionary<string, SKBitmap> ImgCache = new();
}