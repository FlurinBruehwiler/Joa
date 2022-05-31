using Interfaces.Settings;
using Interfaces.Settings.Attributes;

namespace WebSearch;

public class SearchEngine
{
    [SettingProperty] public string Name { get; set; } = string.Empty;

    [SettingProperty] public string Prefix { get; set; } = string.Empty;

    [SettingProperty(Name = "URL")] public string Url { get; set; } = string.Empty;

    [SettingProperty(Name = "Suggestion URL")]
    public string SuggestionUrl { get; set; } = string.Empty;

    [SettingProperty] public IconType IconType { get; set; } = IconType.Svg;
    
    [SettingProperty] public string Icon { get; set; } = string.Empty;

    [SettingProperty] public int Priority { get; set; } = 0;

    [SettingProperty] public bool Fallback { get; set; } = false;

    [SettingProperty] public bool EncodeSearchTerm { get; set; } = true;
}