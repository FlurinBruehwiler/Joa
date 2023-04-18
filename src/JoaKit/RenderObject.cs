using System.ComponentModel;
using SkiaSharp;

namespace JoaKit;

public abstract class RenderObject
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string PFilePath { get; set; } = string.Empty;
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int PLineNumber { get; set; }
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SizeDefinition PWidth { get; set; } = new(100, SizeKind.Percentage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public SizeDefinition PHeight { get; set; } = new(100, SizeKind.Percentage);
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public float PComputedHeight { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public float PComputedWidth { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public float PComputedX { get; set; }
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public float PComputedY { get; set; }
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string? PKey { get; set; }

    public ComponentHash GetComponentHash()
    {
        return new ComponentHash
        {
            Key = PKey,
            FilePath = PFilePath,
            LineNumber = PLineNumber
        };
    }
    
    public abstract void Render(SKCanvas canvas, RenderContext renderContext);
}