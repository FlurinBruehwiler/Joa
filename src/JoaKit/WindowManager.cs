using Microsoft.Extensions.DependencyInjection;
using Modern.WindowKit;
using Modern.WindowKit.Controls.Platform.Surfaces;
using Modern.WindowKit.Platform;
using Modern.WindowKit.Skia;
using SkiaSharp;

namespace JoaKit;

// ReSharper disable once InconsistentNaming
public class WindowManager
{
    private readonly IWindowImpl _window;
    public readonly IComponent RootComponent;
    public readonly Renderer _renderer;
    private SKSurface? _surface;
    public SKImageInfo ImageInfo;
    public SKCanvas? Canvas { get; set; }

    public WindowManager(IWindowImpl window, Type rootType, IServiceProvider serviceProvider, CancellationTokenSource cancellationTokenSource)
    {
        _window = window;
        RootComponent = (IComponent)ActivatorUtilities.CreateInstance(serviceProvider, rootType);
        _renderer = new Renderer(this);
        var inputManager = new InputManager(_renderer, this);
        
        window.Closed = cancellationTokenSource.Cancel;
        
        window.Resized = (_, _) =>
        {
            Canvas?.Dispose();
            Canvas = null;
        };

        window.Input = inputManager.Input;

        window.Paint = DoPaint;

        window.Show(true, false);

        _renderer.Build(RootComponent);
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

        _renderer.LayoutPaintComposite();
    }
    
    private SKSurface GetSurface()
    {
        if (_surface is not null)
            return _surface;

        var screen = _window.ClientSize * _window.RenderScaling;
        var info = new SKImageInfo((int)screen.Width, (int)screen.Height);

        ImageInfo = info;

        _surface = SKSurface.Create(info);
        _surface.Canvas.Clear(SKColors.CornflowerBlue);

        return _surface;
    }
    
    public int Scale(int value) => (int)(value * _window.RenderScaling);
}