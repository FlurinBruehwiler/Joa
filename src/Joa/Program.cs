using JoaCore;
using JoaPluginsPackage.Injectables;
using Microsoft.Extensions.Configuration;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();
if (configuration is null)
{
    JoaLogger.GetInstance().Log("Cant read appsettings.json", IJoaLogger.LogLevel.Error);
    return;
}
var search = new Search(configuration);

JoaLogger.GetInstance().Log("Init Complete", IJoaLogger.LogLevel.Info);


