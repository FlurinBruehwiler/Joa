using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
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
    public InputManager InputManager { get; }
    private List<Component> _componentsToBuld;
    public bool IsBuilding { get; private set; }

    public Builder(WindowManager windowManager, IWindowImpl window)
    {
        _windowManager = windowManager;
        _window = window;
        _layoutEngine = new LayoutEngine(window);
        InputManager = new InputManager(this, windowManager);

        Task.Run(BuildLoop);
    }

    public Component GetComponent(Type componentType)
    {
        var component = (Component)ActivatorUtilities.CreateInstance(_windowManager.JoaKitApp.Services, componentType);
        component.Builder = this;

        return component;
    }

    public void ShouldRebuild(Component component) => _componentsToBuld.Add(component);

    private Component GetSharedRoot()
    {
        if (_componentsToBuld.Count == 1)
            return _componentsToBuld.First();

        var parents = GetParents(_componentsToBuld.First());
        var lastIntersection = 0;

        for (var i = 1; i < _componentsToBuld.Count; i++)
        {
            var component = _componentsToBuld[i];
            var currentIntersection = GetIntersection(parents, component);

            if (currentIntersection > lastIntersection)
                lastIntersection = currentIntersection;
        }

        return parents[lastIntersection];
    }

    private int GetIntersection(List<Component> parents, Component component)
    {
        var current = component;

        while (true)
        {
            if (parents.Contains(current))
                return parents.IndexOf(current);

            current = current.Parent ?? throw new Exception();
        }
    }

    private List<Component> GetParents(Component component)
    {
        var parents = new List<Component>();

        var current = component;
        parents.Add(current);

        while (true)
        {
            if (current.Parent is null)
            {
                break;
            }

            parents.Add(current.Parent);
            current = current.Parent;
        }

        return parents;
    }

    private async Task BuildLoop()
    {
        try
        {
            while (!_windowManager.CancellationToken.IsCancellationRequested)
            {
                var buildStartTime = Stopwatch.GetTimestamp();

                if (_componentsToBuld.Count != 0)
                {
                    IsBuilding = true;

                    var componentToBuld = GetSharedRoot();

                    Build(componentToBuld);

                    IsBuilding = false;

                    _componentsToBuld.Clear();

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

    private void Build(Component componentToBuild)
    {
        _windowManager.JoaKitApp.CurrentlyBuildingWindow = _window;

        var previousRenderObject = componentToBuild.RenderObject;

        var newRenderObject = componentToBuild.Build();
        componentToBuild.RenderObject = newRenderObject;

        AttachNewRenderObject(previousRenderObject, newRenderObject);

        BuildTree(previousRenderObject, newRenderObject);

        _windowManager.JoaKitApp.CurrentlyBuildingWindow = null;
    }

    private void BuildTree(RenderObject oldRenderObject, RenderObject newRenderObject)
    {
        if (oldRenderObject is Div previousDiv && newRenderObject is Div newDiv)
        {
            BuildTreeDiv(previousDiv, newDiv);
        }
    }

    private void BuildTreeDiv(Div previousDiv, Div newDiv)
    {
        var previousRenderObject = new Dictionary<ComponentHash, RenderObject>();

        foreach (var renderObject in previousDiv)
        {
            if (renderObject is CustomRenderObject or Div)
            {
                previousRenderObject.Add(renderObject.GetComponentHash(), renderObject);
            }
        }

        foreach (var renderObject in newDiv)
        {
            if (renderObject is CustomRenderObject customRenderObject)
            {
                if (previousRenderObject.TryGetValue(customRenderObject.GetComponentHash(),
                        out var previousCustomRenderObject))
                {
                    var newRenderObject =
                        customRenderObject.Build(((CustomRenderObject)previousCustomRenderObject).Component);
                    customRenderObject.RenderObject = newRenderObject;
                }
                else
                {
                    BuildNewCustomRenderObject(customRenderObject);
                }
            }
            else if (renderObject is Div div)
            {
                if (previousRenderObject.TryGetValue(div.GetComponentHash(), out var previousDivChild))
                {
                    BuildTreeDiv((Div)previousDivChild, div);
                }
                else
                {
                    BuildNewDiv(div);
                }
            }
        }
    }

    private void BuildNewDiv(Div div)
    {
        foreach (var renderObject in div)
        {
            BuildNewRenderObject(renderObject);
        }
    }

    private void BuildNewCustomRenderObject(CustomRenderObject customRenderObject)
    {
        var renderObject = customRenderObject.Build(GetComponent(customRenderObject.ComponentType));
        customRenderObject.RenderObject = renderObject;

        BuildNewRenderObject(renderObject);
    }

    private void BuildNewRenderObject(RenderObject renderObject)
    {
        if (renderObject is CustomRenderObject customRenderObject)
        {
            BuildNewCustomRenderObject(customRenderObject);
        }
        else if (renderObject is Div childDiv)
        {
            BuildNewDiv(childDiv);
        }
    }

    private void AttachNewRenderObject(RenderObject previousRenderObject, RenderObject newRenderObject)
    {
        var parent = previousRenderObject.Parent;

        switch (parent)
        {
            case Div div:
                {
                    var index = div.Children!.IndexOf(previousRenderObject);
                    div.Children[index] = newRenderObject;
                    break;
                }
            case CustomRenderObject customRenderObject:
                customRenderObject.RenderObject = newRenderObject;
                break;
            default:
                throw new UnreachableException();
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