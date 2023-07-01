using Joa.UI;
using JoaKit;
using JoaKit.Sample;

var builder = JoaKitApp.CreateBuilder();

var app = builder.Build();

app.CreateWindow<UiTest>();

app.Run();