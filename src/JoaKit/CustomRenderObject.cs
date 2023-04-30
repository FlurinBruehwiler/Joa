
using SkiaSharp;

namespace JoaKit;

public abstract class CustomRenderObject : RenderObject
{
    public override void Render(SKCanvas canvas, RenderContext renderContext)
    {
        if (RenderObject is not null)
        {
            RenderObject.Render(canvas, renderContext);
        }
        else
        {
            
        }
    }

    
    public abstract RenderObject Build(Component component);
    public RenderObject? RenderObject { get; set; }
    public Type ComponentType { get; set; }
}