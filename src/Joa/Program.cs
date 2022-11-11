using JoaCore;
using JoaCore.PluginCore;
using JoaCore.Settings;
using JoaInterface;
using JoaInterface.Hubs;
using JoaPluginsPackage.Injectables;
using Microsoft.AspNetCore.Builder;

using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins("http://127.0.0.1:5500", "http://localhost:3000")
            .AllowAnyHeader()
            .WithMethods("GET", "POST")
            .AllowCredentials();
    });
});

builder.Services.AddSingleton<Search>();
builder.Services.AddSingleton<PluginManager>();
builder.Services.AddSingleton<PluginLoader>();
builder.Services.AddSingleton<SettingsManager>();
builder.Services.AddSingleton<SettingsProvider>();
builder.Services.AddSingleton<IJoaLogger>(JoaLogger.GetInstance());
builder.Services.AddSingleton<IIconHelper, IconHelper>();
builder.Services.AddSingleton<PluginServiceProvider>();

builder.Services.AddHostedService<UiManagement>();

var app = builder.Build();

app.MapGet("", () =>
{

});

app.UseCors();
app.MapHub<SearchHub>("/searchHub");
app.MapHub<SettingsHub>("/settingsHub");

app.Run(); 
