using Joa.PluginCore;
using JoaKit;

namespace Joa.UI;

public class SearchResult : IComponent
{
    [Parameter] public PluginSearchResult Sr { get; set; } = default!;

    [Parameter] public bool IsSelected { get; set; } = default!;

    public RenderObject Build()
    {
        var color = IsSelected ? new ColorDefinition(60, 60, 60) : new ColorDefinition(40, 40, 40);

        return new Div
            {
                new Div()
                    .Color(color)
                    .Width(60),
                new Div
                    {
                        new Txt(Sr.SearchResult.Title)
                            .VAlign(TextAlign.Center)
                            .Size(15)
                    }.MAlign(MAlign.Center)
                    .Color(color)
            }
            .Dir(Dir.Horizontal);
    }
}