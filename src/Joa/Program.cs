﻿using JoaInterface;
using JoaInterface.Hubs;
using JoaInterface.Injectables;
using JoaInterface.PluginCore;
using JoaInterface.Settings;
using JoaInterface.Step;
using Joa.Api.Injectables;
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
builder.Services.AddSingleton<IJoaLogger>(JoaLogger.GetInstance());
builder.Services.AddSingleton<PluginServiceProvider>();
builder.Services.AddSingleton<StepsManager>();

builder.Services.AddHostedService<UiManagement>();

var app = builder.Build();

app.Services.GetService<Search>();

app.UseCors();
app.MapHub<SearchHub>("/searchHub");
app.MapHub<SettingsHub>("/settingsHub");

app.Run(); 
