namespace JoaLauncher.Api.Enums;

public enum OperatingSystem
{
    Windows = 0x0001,
    Linux = 0x0002,
    MacOS = 0x0004,
    All = Windows | Linux | MacOS
}