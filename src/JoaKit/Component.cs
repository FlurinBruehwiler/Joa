namespace JoaKit;

public abstract class Component
{
    public abstract RenderObject Build();

    public Renderer Renderer { get; set; } = null!;
    public void StateHasChanged()
    {
        Renderer.ShouldRebuild();
    }
}