using JoaLauncher.Api;
using Microsoft.Extensions.Options;

namespace Joa;

public class SettingOption : IOptions<ISetting>
{
    public SettingOption(ISetting value)
    {
        Value = value;
    }

    public ISetting Value { get; set; }
}