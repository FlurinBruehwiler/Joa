using System.Diagnostics;
using Modern.WindowKit;
using Modern.WindowKit.Platform;
using Modern.WindowKit.Threading;
using SkiaSharp;

namespace JoaKit;

public class Builder
{
    private readonly WindowManager _windowManager;
    private readonly IWindowImpl _window;
    private readonly LayoutEngine _layoutEngine;
    private readonly BuildContext _buildContext;
    public InputManager InputManager { get; }
    private bool _shouldRebuild;
    public bool IsBuilding { get; private set; }

    public Builder(WindowManager windowManager, IWindowImpl window)
    {
        _windowManager = windowManager;
        _window = window;
        _layoutEngine = new LayoutEngine(window);
        _buildContext = new BuildContext(_windowManager.JoaKitApp.Services);
        InputManager = new InputManager(this, windowManager);
        
        Task.Run(BuildLoop);
    }
    
    public void ShouldRebuild() => _shouldRebuild = true;

    private async Task BuildLoop()
    {
        try
        {
            while (!_windowManager.CancellationToken.IsCancellationRequested)
            {
                var buildStartTime = Stopwatch.GetTimestamp();

                if (_shouldRebuild)
                {
                    _shouldRebuild = false;
                    IsBuilding = true;
                
                    Build(_windowManager.RootComponent);
                    IsBuilding = false;

                    await Dispatcher.UIThread.InvokeAsync(() => _windowManager.DoPaint(new Rect()));
                }
            
                var totalBuildTime = Stopwatch.GetElapsedTime(buildStartTime).TotalMilliseconds;
                const int frameTime = 1000 / 60;

                if (totalBuildTime < frameTime)
                {
                    var timeToWait = frameTime - totalBuildTime;
                    await Task.Delay((int)timeToWait, _windowManager.CancellationToken);
                }
            }
        }
        catch (Exception e)
        {
            // JoaLogger.Instance.LogError(e, "Renderer");
            //ToDo
        }
    }

    private static readonly SKPaint SPaint = new()
    {
        IsAntialias = true
    };

    public static SKPaint GetColor(ColorDefinition colorDefinition)
    {
        SPaint.Color = new SKColor((byte)colorDefinition.Red, (byte)colorDefinition.Gree, (byte)colorDefinition.Blue,
            (byte)colorDefinition.Transparency);
        return SPaint;
    }


    public RenderObject Root = null!;

    private Div? _clickedElement;


    private void Build(Component rootComponent)
    {
        _windowManager.JoaKitApp.CurrentlyBuildingWindow = _window;
        
        Root = rootComponent.Build();
        BuildTree(Root, _buildContext);

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
                        var component = buildContext.GetComponent(componentHash, customRenderObject.ComponentType, this);
                        var childRenderObject = customRenderObject.Build(component);
                        renderObject = childRenderObject;
                        continue;
                    }
                case Div div:
                    {
                        if (div.PAutoFocus)
                        {
                            InputManager.ActiveDiv = div;
                        }
                        
                        if(div.Children is null)
                            break;
                        
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
        if (Root is null)
            return;
        
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