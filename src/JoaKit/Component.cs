namespace JoaKit;

public abstract class Component
{
    public abstract RenderObject Build();

    public Builder Builder { get; set; } = null!;
    public void StateHasChanged()
    {
        Builder.ShouldRebuild();
    }
}