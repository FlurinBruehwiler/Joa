﻿using Microsoft.Extensions.DependencyInjection;
using Modern.WindowKit;
using Modern.WindowKit.Controls.Platform.Surfaces;
using Modern.WindowKit.Platform;
using Modern.WindowKit.Skia;
using SkiaSharp;

namespace JoaKit;

public class WindowManager
{
    private readonly IWindowImpl _window;
    public readonly IComponent RootComponent;
    public readonly Renderer Renderer;
    private SKSurface? _surface;
    public ServiceProvider ServiceProvider { get; }
    public SKCanvas? Canvas { get; set; }

    public WindowManager(IWindowImpl window, Type rootType, IServiceCollection serviceCollection, CancellationTokenSource cancellationTokenSource)
    {
        _window = window;

        serviceCollection.AddSingleton(window);

        ServiceProvider = serviceCollection.BuildServiceProvider();
        
        RootComponent = (IComponent)ActivatorUtilities.CreateInstance(ServiceProvider, rootType);
        Renderer = new Renderer(this, window);
        var inputManager = new InputManager(Renderer, this);
        
        window.Closed = cancellationTokenSource.Cancel;
        
        window.Resized = (_, _) =>
        {
            Canvas?.Dispose();
            Canvas = null;
        };

        window.Input = inputManager.Input;

        window.Paint = DoPaint;

        window.Show(true, false);

        Renderer.Build(RootComponent);
    }
    
    public void DoPaint(Rect bounds)
    {
        var skiaFramebuffer = _window.Surfaces.OfType<IFramebufferPlatformSurface>().First();

        using var framebuffer = skiaFramebuffer.Lock();

        var framebufferImageInfo = new SKImageInfo(framebuffer.Size.Width, framebuffer.Size.Height,
            framebuffer.Format.ToSkColorType(),
            framebuffer.Format == PixelFormat.Rgb565 ? SKAlphaType.Opaque : SKAlphaType.Premul);

        using var surface = SKSurface.Create(framebufferImageInfo, framebuffer.Address, framebuffer.RowBytes);

        surface.Canvas.DrawSurface(GetSurface(), SKPoint.Empty);
        Canvas = surface.Canvas;

        Renderer.LayoutPaintComposite(_window.ClientSize * _window.RenderScaling);
    }
    
    private SKSurface GetSurface()
    {
        if (_surface is not null)
            return _surface;

        var screen = _window.ClientSize * _window.RenderScaling;
        var info = new SKImageInfo((int)screen.Width, (int)screen.Height);
        
        _surface = SKSurface.Create(info);
        _surface.Canvas.Clear(SKColors.CornflowerBlue);

        return _surface;
    }
    
    public int Scale(int value) => (int)(value * _window.RenderScaling);
}

public static class WindowExtensions
{
    public static float Scale(this IWindowImpl window, float value)
    {
        return (float)(value * window.RenderScaling);
    }
}
