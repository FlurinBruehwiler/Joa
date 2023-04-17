namespace JoaKit;

public abstract class CustomRenderObject : RenderObject
{
    public abstract RenderObject Build(IComponent component);
    public RenderObject? RenderObject { get; set; }
    public Type ComponentType { get; set; }
}