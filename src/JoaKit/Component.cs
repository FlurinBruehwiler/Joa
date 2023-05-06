namespace JoaKit;

public abstract class Component
{
    public abstract RenderObject Build();
    public RenderObject RenderObject { get; set; }
    public Component? Parent { get; set; }
    public Builder Builder { get; set; } = null!;
    public void StateHasChanged()
    {
        Builder.ShouldRebuild(this);
    }
}