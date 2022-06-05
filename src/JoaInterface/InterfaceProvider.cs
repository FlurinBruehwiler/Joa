using JoaCore;

namespace JoaInterface;

public class InterfaceProvider
{
    public InterfaceProvider(Search search)
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
        builder.Services.AddSingleton(search);
        var app = builder.Build();
        app.UseCors();
        app.MapHub<SearchHub>("/searchHub");
        app.Run();
    }
}