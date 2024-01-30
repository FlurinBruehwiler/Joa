using Flamui;
using Flamui.UiElements;
using Joa.PluginCore;
using Joa.Settings;

namespace Joa.UI.Settings;

public class PropertyDisplay : FlamuiComponent
{
    private readonly SettingsManager _settingsManager;

    [Parameter]
    public PluginDefinition PluginDefinition { get; set; } = null!;

    [Parameter]
    public PropertyInstance Property { get; set; } = null!;

    public PropertyDisplay(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public override void Build()
    {
        var type = Property.PropertyDescription.PropertyInfo.PropertyType;

        if (type == typeof(int) || type == typeof(float) || type == typeof(string))
        {
            DivStart().Height(50).Color(36, 36, 36).XAlign(XAlign.Center).Dir(Dir.Horizontal).Padding(8).MAlign(MAlign.SpaceBetween).Rounded(7);

                Text(Property.PropertyDescription.PropertyInfo.Name).VAlign(TextAlign.Center).Size(17);
                var x = "";
                StyledInput(ref x);
            DivEnd();
        }

        if (type == typeof(bool))
        {
            DivStart().Height(50).Color(36, 36, 36).Padding(8).Rounded(7).XAlign(XAlign.Center).Dir(Dir.Horizontal);
                Text(Property.PropertyDescription.PropertyInfo.Name).VAlign(TextAlign.Center).Size(17);

                var x = true;
                Checkbox(ref x);

            DivEnd();
        }
    }

    private async Task PropertyChanged(string arg)
    {
        Property.SetValue(arg);
        await _settingsManager.SaveSettingsToJsonAsync();
    }
}
