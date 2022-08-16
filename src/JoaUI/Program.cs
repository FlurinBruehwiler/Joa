using ElectronNET.API;
using ElectronNET.API.Entities;
using ElectronNetTest.Shared;
using JoaCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseElectron(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<JoaSearch>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
{
    Width = 600,
    Height = 60,
    TitleBarStyle = TitleBarStyle.hidden,
    AutoHideMenuBar = true,
    SkipTaskbar = true
});

app.Run();