using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JoaKit;

public class JoaKitBuilder
{
    public JoaKitBuilder()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public JoaKitApp Build()
    {
        return new JoaKitApp(Services);
    }

    public IConfiguration Configuration { get; private set; }
    public IServiceCollection Services { get; } = new ServiceCollection();
}