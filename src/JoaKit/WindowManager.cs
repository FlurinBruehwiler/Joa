using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modern.WindowKit;
using Modern.WindowKit.Controls.Platform.Surfaces;
using Modern.WindowKit.Platform;
using Modern.WindowKit.Skia;
using SkiaSharp;

namespace JoaKit;

public class WindowManager
{
    public JoaKitApp JoaKitApp { get; }
    public IWindowImpl Window { get; }
    public readonly Component RootComponent;
    public readonly Builder Builder;
    private SKSurface? _surface;
    public SKCanvas? Canvas { get; set; }
    public CancellationToken CancellationToken { get; }

    public WindowManager(JoaKitApp joaKitApp, IWindowImpl window, Type rootType, CancellationTokenSource cancellationTokenSource)
    {
        CancellationToken = cancellationTokenSource.Token;
        JoaKitApp = joaKitApp;
        Window = window;
        joaKitApp.CurrentlyBuildingWindow = window;

        Builder = new Builder(this, window);

        RootComponent = (Component)ActivatorUtilities.CreateInstance(JoaKitApp.Services, rootType);
        RootComponent.Builder = Builder;

        joaKitApp.CurrentlyBuildingWindow = null;

        window.Closed = cancellationTokenSource.Cancel;

        window.Resized = (_, _) =>
        {
            Canvas?.Dispose();
            Canvas = null;
        };

        window.Input = Builder.InputManager.Input;

        window.Paint = DoPaint;

        Builder.ShouldRebuild(RootComponent);
    }

    public void DoPaint(Rect bounds)
    {
        JoaLogger.GetInstance().LogInformation("Repainting");

        var skiaFramebuffer = Window.Surfaces.OfType<IFramebufferPlatformSurface>().First();

        using var framebuffer = skiaFramebuffer.Lock();

        var framebufferImageInfo = new SKImageInfo(framebuffer.Size.Width, framebuffer.Size.Height,
            framebuffer.Format.ToSkColorType(),
            framebuffer.Format == PixelFormat.Rgb565 ? SKAlphaType.Opaque : SKAlphaType.Premul);

        using var surface = SKSurface.Create(framebufferImageInfo, framebuffer.Address, framebuffer.RowBytes);

        // surface.Canvas.DrawSurface(GetSurface(), SKPoint.Empty);
        Canvas = surface.Canvas;

        
        Builder.LayoutPaintComposite(Window.ClientSize * Window.RenderScaling);
        
        var newSurface = SKSurface.Create(new SKImageInfo(framebuffer.Size.Width, framebuffer.Size.Height));
        // newSurface.Canvas.Translate(100, 0);
        newSurface.Canvas.DrawRect(0, 0, 100, 100, new SKPaint
        {
            Color = SKColors.Black
        });
        // newSurface.Canvas.SetMatrix(new SKMatrix(0, 0, 50, 0, 0, 100, 0, 0, 0));

        
        
        Canvas.DrawSurface(newSurface, 0, 0);
        
    }

    public int Scale(int value) => (int)(value * Window.RenderScaling);
}

public static class WindowExtensions
{
    public static float Scale(this IWindowImpl window, float value)
    {
        return (float)(value * window.RenderScaling);
    }
}
