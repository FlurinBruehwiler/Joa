using JoaKit;

namespace Joa.UI;

public class SearchResult : IComponent
{
    [Parameter] public string Text { get; set; } = string.Empty;
    
    public RenderObject Build()
    {
        return new Div
        {
            new Txt(Text)
        }.Color(40, 40, 40)
            .BorderWidth(2);
    }
}