using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;

namespace BookmarksSearch;

public class BookmarksSearchSetting : ISetting
{
    [SettingProperty]
    public List<Browser> Browsers { get; set; } = new()
    {
        DefaultBrowsers.Chrome,
        DefaultBrowsers.Firefox,
        DefaultBrowsers.Brave,
        DefaultBrowsers.Edge
    };
}