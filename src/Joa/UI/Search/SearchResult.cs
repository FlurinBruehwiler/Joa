using Flamui;
using Flamui.UiElements;
using Joa.PluginCore;

namespace Joa.UI.Search;

public class SearchResult : FlamuiComponent
{
    [Parameter]
    public PluginSearchResult Sr { get; set; } = default!;

    [Parameter]
    public bool IsSelected { get; set; } = default!;

    public override void Build()
    {
        var color = IsSelected ? new ColorDefinition(60, 60, 60) : new ColorDefinition(40, 40, 40);

        DivStart().Dir(Dir.Horizontal);

            DivStart().Color(color).Width(60).MAlign(MAlign.Center).XAlign(XAlign.Center);
                Image(Sr.SearchResult.Icon);
            DivEnd();

            DivStart().MAlign(MAlign.Center).Color(color);
                Text(Sr.SearchResult.Title).VAlign(TextAlign.Center).Size(15);
            DivEnd();

        DivEnd();
    }
}
