using JoaLauncher.Api.Injectables;
using JoaInterface;
using JoaInterface.Hubs;
using JoaInterface.Injectables;
using JoaInterface.PluginCore;
using JoaInterface.Settings;
using JoaInterface.Step;
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


builder.Services.AddSingleton<IJoaLogger>(JoaLogger.GetInstance());
builder.Services.AddSingleton<JoaManager>();

builder.Services.Configure<PathsConfiguration>(builder.Configuration.GetSection("Paths"));

builder.Services.AddHostedService<UiManagement>();

var app = builder.Build();



app.UseCors();
app.MapHub<SearchHub>("/searchHub");
app.MapHub<SettingsHub>("/settingsHub");

app.Run(); 
