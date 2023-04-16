using SkiaSharp;

namespace JoaKit;

public class Renderer
{
    private readonly WindowManager _windowManager;

    public Renderer(WindowManager windowManager)
    {
        _windowManager = windowManager;
    }
    
    private static readonly SKPaint s_paint = new()
    {
        IsAntialias = true
    };

    public static SKPaint GetColor(ColorDefinition colorDefinition)
    {
        s_paint.Color = new SKColor((byte) colorDefinition.Red, (byte)colorDefinition.Gree, (byte)colorDefinition.Blue, (byte)colorDefinition.Transparency);
        return s_paint;
    }

    private LayoutEngine _layoutEngine = new();

    private readonly Div _oldRoot = new();
    
    public RenderObject NewRoot = null!;

    private Div? _clickedElement;

    public void Build(UiComponent rootComponent)
    {
        NewRoot = rootComponent.Render();
    }
    
    public void LayoutPaintComposite()
    {
        var wrapper = new Div
        {
            NewRoot
        }.Width(_windowManager.ImageInfo.Width).Height(_windowManager.ImageInfo.Height);
        wrapper.PComputedHeight = _windowManager.ImageInfo.Height;
        wrapper.PComputedWidth = _windowManager.ImageInfo.Width;

        _layoutEngine.ApplyLayoutCalculations(wrapper, _oldRoot);

        wrapper.Render(_windowManager.Canvas!);

        _clickedElement?.POnClick?.Invoke();
        _clickedElement = null;
    }
}