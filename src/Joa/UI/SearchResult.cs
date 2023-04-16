using JoaKit;

namespace Joa.UI;

public class SearchResult : IComponent
{
    [Parameter] public string Text { get; set; } = string.Empty;
    
    public RenderObject Render()
    {
        return new Div
        {
            new Txt(Text)
        };
    }
}