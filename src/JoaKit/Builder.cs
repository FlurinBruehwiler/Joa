using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    private readonly List<Component> _componentsToBuld = new();

    public Builder(WindowManager windowManager, IWindowImpl window)
    {
        _windowManager = windowManager;
        _window = window;
        _layoutEngine = new LayoutEngine(window);
        InputManager = new InputManager(this, windowManager);

        Task.Run(BuildLoop);
    }

    private Component GetComponent(Type componentType, Component parent)
    {
        var component = (Component)ActivatorUtilities.CreateInstance(_windowManager.JoaKitApp.Services, componentType);
        component.Builder = this;
        component.Parent = parent;

        return component;
    }

    public void ShouldRebuild(Component component) => _componentsToBuld.Add(component);

    private Component GetSharedRoot(List<Component> components)
    {
        if (components.Count == 1)
            return components.First();

        var parents = GetParents(components.First());
        var lastIntersection = 0;

        for (var i = 1; i < components.Count; i++)
        {
            var component = components[i];
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
                    var copy = new List<Component>(_componentsToBuld);
                    _componentsToBuld.Clear();
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        var componentToBuld = GetSharedRoot(copy);
                        Build(componentToBuld);
                        _windowManager.DoPaint(new Rect());
                    });
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


    public RenderObject? Root;
    public Component RootComponent;

    private void Build(Component componentToBuild)
    {
        JoaLogger.GetInstance().LogInformation($"Building {componentToBuild.GetType().Name}");

        _windowManager.JoaKitApp.CurrentlyBuildingWindow = _window;
        InputManager.AbsoluteDivs.Clear();

        if (Root is null)
        {
            RootComponent = componentToBuild;
            var renderObject = componentToBuild.Build();
            componentToBuild.RenderObject = renderObject;
            Root = renderObject;
            BuildNewRenderObject(renderObject, componentToBuild);
        }
        else
        {
            var previousRenderObject = componentToBuild.RenderObject;

            if (previousRenderObject is null)
            {
                Debugger.Break();
            }

            var newRenderObject = componentToBuild.Build();
            componentToBuild.RenderObject = newRenderObject;

            AttachNewRenderObject(previousRenderObject, newRenderObject);

            BuildTree(previousRenderObject, newRenderObject, componentToBuild);
        }

        ValidateTree(Root);
        _windowManager.JoaKitApp.CurrentlyBuildingWindow = null;
    }

    private void ValidateTree(RenderObject renderObject)
    {
        if (renderObject != Root)
        {
            if (renderObject.Parent is null)
            {
                Debugger.Break();
            }
        }

        if (renderObject is Div div)
        {
            if (div.PAbsolute)
            {
                InputManager.AbsoluteDivs.Add(div);
            }

            foreach (var child in div)
            {
                ValidateTree(child);
            }
        }
        else if (renderObject is CustomRenderObject customRenderObject)
        {
            if (customRenderObject.Component is null)
            {
                Debugger.Break();
            }

            ValidateTree(customRenderObject.RenderObject);
        }
    }

    private void BuildTree(RenderObject oldRenderObject, RenderObject newRenderObject, Component parent)
    {
        if (oldRenderObject is Div previousDiv && newRenderObject is Div newDiv)
        {
            BuildTreeDiv(previousDiv, newDiv, parent);
        }
    }

    private void BuildTreeDiv(Div previousDiv, Div newDiv, Component parent)
    {
        var previousRenderObject = new Dictionary<ComponentHash, RenderObject>();

        newDiv.IsHovered = previousDiv.IsHovered;

        if (newDiv.IsActive)
        {
            InputManager.ActiveDiv = newDiv;
        }

        if (newDiv.IsHovered)
        {
            InputManager.HoveredDiv = newDiv;
        }

        foreach (var renderObject in previousDiv)
        {
            if (renderObject is CustomRenderObject or Div)
            {
                previousRenderObject.Add(renderObject.GetComponentHash(), renderObject);
            }
        }

        foreach (var renderObject in newDiv)
        {
            renderObject.Parent = newDiv;

            if (renderObject is CustomRenderObject customRenderObject)
            {
                if (previousRenderObject.TryGetValue(customRenderObject.GetComponentHash(),
                        out var previousCustomRenderObject))
                {
                    var previousActuallyCustomRenderObject = (CustomRenderObject)previousCustomRenderObject;
                    var component = previousActuallyCustomRenderObject.Component;
                    var newRenderObject = customRenderObject.Build(component);
                    newRenderObject.Parent = customRenderObject;
                    customRenderObject.RenderObject = newRenderObject;
                    customRenderObject.Component = component;
                    BuildTree(previousActuallyCustomRenderObject.RenderObject, newRenderObject, component);
                }
                else
                {
                    BuildNewCustomRenderObject(customRenderObject, parent);
                }
            }
            else if (renderObject is Div div)
            {
                if (previousRenderObject.TryGetValue(div.GetComponentHash(), out var previousDivChild))
                {
                    BuildTreeDiv((Div)previousDivChild, div, parent);
                }
                else
                {
                    BuildNewDiv(div, parent);
                }
            }
        }
    }

    private void BuildNewDiv(Div div, Component parent)
    {
        if (div.PAutoFocus)
        {
            InputManager.ActiveDiv = div;
        }

        foreach (var renderObject in div)
        {
            renderObject.Parent = div;
            BuildNewRenderObject(renderObject, parent);
        }
    }

    private void BuildNewCustomRenderObject(CustomRenderObject customRenderObject, Component parent)
    {
        var newComponent = GetComponent(customRenderObject.ComponentType, parent);
        var renderObject = customRenderObject.Build(newComponent);
        customRenderObject.RenderObject = renderObject;
        customRenderObject.Component = newComponent;
        newComponent.RenderObject = renderObject;
        renderObject.Parent = customRenderObject;

        BuildNewRenderObject(renderObject, newComponent);
    }

    private void BuildNewRenderObject(RenderObject renderObject, Component parent)
    {
        if (renderObject is CustomRenderObject customRenderObject)
        {
            BuildNewCustomRenderObject(customRenderObject, parent);
        }
        else if (renderObject is Div childDiv)
        {
            BuildNewDiv(childDiv, parent);
        }
    }

    private void AttachNewRenderObject(RenderObject previousRenderObject, RenderObject newRenderObject)
    {
        var parent = previousRenderObject.Parent;

        if (parent is null)
        {
            Root = newRenderObject;
            return;
        }

        switch (parent)
        {
            case Div div:
                {
                    var index = div.Children!.IndexOf(previousRenderObject);
                    div.Children[index] = newRenderObject;
                    newRenderObject.Parent = div;
                    break;
                }
            case CustomRenderObject customRenderObject:
                customRenderObject.RenderObject = newRenderObject;
                newRenderObject.Parent = customRenderObject;
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
    }
}