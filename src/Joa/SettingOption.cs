using JoaLauncher.Api;

namespace Joa;

public class SettingOption : IOptions<ISetting>
{
    public SettingOption(ISetting value)
    {
        Value = value;
    }

    public ISetting Value { get; set; }
}