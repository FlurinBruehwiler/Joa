using Flamui;
using Flamui.UiElements;
using Joa.PluginCore;

namespace Joa.UI.Settings;

public partial class SettingsWindow : FlamuiComponent
{
    private readonly PluginManager _pluginManager;
    private PluginDefinition? _selectedPlugin;

    public SettingsWindow(PluginManager pluginManager)
    {
        _pluginManager = pluginManager;
        _selectedPlugin = _pluginManager.Plugins.FirstOrDefault();
    }

    private string _input = "Test";

    public override void Build()
    {
        DivStart().Dir(Dir.Horizontal);

            DivStart().Width(300).Color(40, 40, 40);

                DivStart().Height(70);
                DivEnd();

                DivStart().Gap(6).Padding(15);

                    foreach (var pluginManagerPlugin in _pluginManager.Plugins)
                    {
                        DivStart(out var pluginEntry, pluginManagerPlugin.Manifest.Id).Height(40).Color(62, 62, 62)
                            .Padding(10).Rounded(4);

                        if (pluginEntry.IsHovered)
                        {
                            pluginEntry.Color(70, 70, 70);
                        }

                        if (pluginEntry.IsClicked)
                        {
                            _selectedPlugin = pluginManagerPlugin;
                        }

                        DivEnd();
                    }

                DivEnd();

                DivStart().Height(93);

                    if (_selectedPlugin is not null)
                    {
                        DivStart().Height(100).Padding(35);
                            DivStart();
                                Text(_selectedPlugin.Manifest.Name);
                            DivEnd();

                            DivStart();
                            Text(_selectedPlugin.Manifest.Description ?? string.Empty);
                            DivEnd();
                        DivEnd();

                        DivStart().Padding(30).Gap(10);

                        foreach (var property in _selectedPlugin.SettingConfiguration.PropertyInstances)
                        {
                            //todo display property
                        }

                        DivEnd();

                    }

                DivEnd();

            DivEnd();


            DivStart().Color(20, 20, 20);
            DivEnd();


        DivEnd();
    }
}
