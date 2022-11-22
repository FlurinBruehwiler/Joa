using Joa;
using Joa.Hubs;
using Joa.Injectables;
using Joa.PluginCore;
using Joa.Settings;
using Joa.Step;
using JoaLauncher.Api.Injectables;
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
builder.Services.AddScoped<Search>();
builder.Services.AddScoped<PluginManager>();
builder.Services.AddScoped<PluginLoader>();
builder.Services.AddScoped<SettingsManager>();
builder.Services.AddScoped<PluginServiceProvider>();
builder.Services.AddScoped<StepsManager>();
builder.Services.AddSingleton<FileSystemManager>();
builder.Services.Configure<PathsConfiguration>(builder.Configuration.GetSection("Paths"));
builder.Services.Configure<ReflectionConfiguration>(builder.Configuration.GetSection("Reflection"));

// builder.Services.AddHostedService<UiManagement>();

var app = builder.Build();

app.Services.GetRequiredService<JoaManager>();


app.UseCors();
app.MapHub<SearchHub>("/searchHub");
app.MapHub<SettingsHub>("/settingsHub");

app.Run(); 
