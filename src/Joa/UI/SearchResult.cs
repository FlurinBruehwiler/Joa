using Joa.PluginCore;
using JoaKit;

namespace Joa.UI;

public class SearchResult : IComponent
{
    [Parameter] 
    public PluginSearchResult Sr { get; set; } = default!;
    
    public RenderObject Build()
    {
        return new Div
        {
            new Div()
                .Color(40, 40, 40)
                .Width(60),
            new Div
            {
                new Txt(Sr.SearchResult.Title)
                    .VAlign(TextAlign.Center)
                    .Size(15)
            }.MAlign(MAlign.Center)
                .Color(40, 40, 40)

        }
            .Dir(Dir.Row);
    }
}