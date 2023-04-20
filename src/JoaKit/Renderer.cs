using Modern.WindowKit;
using Modern.WindowKit.Platform;
using SkiaSharp;

namespace JoaKit;

public class Renderer
{
    private readonly WindowManager _windowManager;
    private readonly IWindowImpl _window;
    private readonly LayoutEngine _layoutEngine;

    public Renderer(WindowManager windowManager, IWindowImpl window)
    {
        _windowManager = windowManager;
        _window = window;
        _layoutEngine = new LayoutEngine(window);
    }

    private static readonly SKPaint s_paint = new()
    {
        IsAntialias = true
    };

    public static SKPaint GetColor(ColorDefinition colorDefinition)
    {
        s_paint.Color = new SKColor((byte)colorDefinition.Red, (byte)colorDefinition.Gree, (byte)colorDefinition.Blue,
            (byte)colorDefinition.Transparency);
        return s_paint;
    }


    public RenderObject Root = null!;

    private Div? _clickedElement;


    public void Build(IComponent rootComponent)
    {
        _windowManager.JoaKitApp.CurrentlyBuildingWindow = _window;
        
        var buildContext = new BuildContext(_windowManager.JoaKitApp.Services);
        
        Root = rootComponent.Build();
        BuildTree(Root, buildContext);
        
        _windowManager.JoaKitApp.CurrentlyBuildingWindow = null;
    }

    private void BuildTree(RenderObject renderObject, BuildContext buildContext)
    {
        while (true)
        {
            switch (renderObject)
            {
                case CustomRenderObject customRenderObject:
                {
                    var componentHash = customRenderObject.GetComponentHash();
                    var component = buildContext.GetComponent(componentHash, customRenderObject.ComponentType);
                    var childRenderObject = customRenderObject.Build(component);
                    renderObject = childRenderObject;
                    continue;
                }
                case Div { Children.Count: > 0 } div:
                {
                    foreach (var divChild in div.Children)
                    {
                        BuildTree(divChild, buildContext);
                    }

                    break;
                }
            }

            break;
        }
    }

    public void LayoutPaintComposite(Size windowRenderScaling)
    {
        var wrapper = new Div
        {
            Root
        };
        wrapper.PComputedHeight = (float)windowRenderScaling.Height;
        wrapper.PComputedWidth = (float)windowRenderScaling.Width;

        _layoutEngine.ApplyLayoutCalculations(wrapper);
        wrapper.Render(_windowManager.Canvas!, new RenderContext(_window));

        _clickedElement?.POnClick?.Invoke();
        _clickedElement = null;
    }
}