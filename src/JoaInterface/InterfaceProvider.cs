using JoaCore;
using JoaCore.Settings;
using JoaInterface.HotKey;
using JoaInterface.Hubs;
using JoaInterface.MousePosition;
using Microsoft.AspNetCore.SignalR;

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
        HotKeyHelper.RegisterHotKey(() =>
        {
            if (SearchHub.GlobalContext is null)
                return;
            var mousePos = MousePositionHelper.GetMousePosition();
            SearchHub.GlobalContext.Clients.All.SendAsync("ShowWindow", mousePos.X, mousePos.Y);
        }, Key.P, Modifier.Alt);
        
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