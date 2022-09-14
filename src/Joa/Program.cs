using JoaCore;
using JoaCore.PluginCore;
using JoaCore.Settings;
using JoaInterface;
using JoaInterface.Hubs;
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
builder.Services.AddSingleton<CoreSettings>();
builder.Services.AddSingleton(JoaLogger.GetInstance());
builder.Services.AddSingleton<ServiceProviderForPlugins>();

builder.Services.AddHostedService<UiManagement>();

var app = builder.Build();

app.UseCors();
app.MapHub<SearchHub>("/searchHub");
app.MapHub<SettingsHub>("/settingsHub");

app.Run(); 
