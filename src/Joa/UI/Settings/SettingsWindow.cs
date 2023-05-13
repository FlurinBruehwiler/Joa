using System.Security.Principal;
using Joa.PluginCore;
using JoaKit;

namespace Joa.UI.Settings;

public class SettingsWindow : Component
{
    private readonly PluginManager _pluginManager;
    private PluginDefinition _selectedPlugin;

    public SettingsWindow(PluginManager pluginManager)
    {
        _pluginManager = pluginManager;
        _selectedPlugin = _pluginManager.Plugins.First();
    }

    private string _input = "Test";

    public override RenderObject Build()
    {
        return new Div
        {
            new Div
                {
                    new Div()
                        .Height(70),
                    new Div()
                        .Items(_pluginManager.Plugins.Select(x =>
                            new Div
                                {
                                    new Txt(x.Manifest.Name).VAlign(TextAlign.Center)
                                }.Height(40)
                                .Color(62, 62, 62)
                                .HoverColor(70, 70, 70)
                                .Padding(10)
                                .Radius(4)
                                .OnClick(() => _selectedPlugin = x)
                                .Key(x.Manifest.Id)))
                        .Gap(6)
                        .Padding(15),
                    new Div()
                        .Height(93)
                }
                .Width(300)
                .Color(40, 40, 40),
            new Div
            {
                new Div
                    {
                        new Div
                        {
                            new Txt(_selectedPlugin.Manifest.Name)
                        },
                        new Div
                        {
                            new Txt(_selectedPlugin.Manifest.Description ?? string.Empty)
                        }
                    }
                    .Height(100)
                    .Padding(35),
                new Div()
                    .Items(
                    _selectedPlugin.SettingConfiguration.PropertyInstances.Select((property, i) =>
                        new PropertyDisplayComponent(_selectedPlugin, property).Key(i.ToString())))
                    .Padding(30)
                    .Gap(10)
            }.Color(20, 20, 20)
        }.Dir(Dir.Horizontal);
    }
}