using Joa.PluginCore;
using JoaKit;

namespace Joa.UI;

public class SearchResult : IComponent
{
    [Parameter] public PluginSearchResult Sr { get; set; } = default!;
    
    public RenderObject Build()
    {
        return new Div
        {
            new Txt(Sr.SearchResult.Title)
        }.Color(40, 40, 40)
            .BorderWidth(2);
    }
}