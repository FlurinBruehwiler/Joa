using JoaCore;
using JoaInterface.Hubs;

namespace JoaInterface;

public class InterfaceProvider
{
    private readonly Search _search;

    public InterfaceProvider(Search search)
    {
        _search = search;
    }

    public void Run()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSignalR();
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder =>
            {
                policyBuilder.WithOrigins("http://127.0.0.1:5500", "http://localhost:3000").AllowAnyHeader().WithMethods("GET", "POST").AllowCredentials();
            });
        });
        builder.Services.AddSingleton(_search);
        builder.Services.AddSingleton(_search.SettingsManager);
        var app = builder.Build();
        app.UseCors();
        app.MapHub<SearchHub>("/searchHub");
        app.MapHub<SettingsHub>("/settingsHub");
        app.Run(); 
    }
}