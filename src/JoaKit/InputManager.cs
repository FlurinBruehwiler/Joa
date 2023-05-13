using System.Diagnostics;
using Modern.WindowKit;
using Modern.WindowKit.Input;
using Modern.WindowKit.Input.Raw;
using Modern.WindowKit.Threading;

namespace JoaKit;

public class InputManager
{
    private readonly Builder _builder;
    private readonly WindowManager _windowManager;
    private Div? _activeDiv;
    private Div? _hoveredDiv;

    public Div? ActiveDiv
    {
        get => _activeDiv;
        set
        {
            if (ActiveDiv is not null)
            {
                ActiveDiv.IsActive = false;
            }
            
            _activeDiv = value;
            if (value is not null)
            {
                value.IsActive = true;
            }
        }
    }

    public Div? HoveredDiv
    {
        get => _hoveredDiv;
        set
        {
            if (HoveredDiv is not null)
            {
                HoveredDiv.IsHovered = false;
            }
            
            _hoveredDiv = value;
            if (value is not null)
            {
                value.IsHovered = true;
            }
        }
    }
    

    public InputManager(Builder builder, WindowManager windowManager)
    {
        _builder = builder;
        _windowManager = windowManager;
    }

    public void Input(RawInputEventArgs args)
    {
        var objectsWhichReceivedACallback = new HashSet<RenderObject>();

        if (args is RawKeyEventArgs { Type: RawKeyEventType.KeyDown } keyEventArgs)
        {
            if (keyEventArgs.Key == Key.F5)
            {
                _builder.ShouldRebuild(_windowManager.RootComponent);
            }

            if (ActiveDiv?.POnKeyDown is not null)
            {
                ActiveDiv.POnKeyDown(keyEventArgs.Key, keyEventArgs.Modifiers);
                objectsWhichReceivedACallback.Add(ActiveDiv);
            }

            if (ActiveDiv?.POnKeyDownAsync is not null)
            {
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    if (Environment.CurrentManagedThreadId != 1)
                    {
                        Debugger.Break();
                    }

                    await ActiveDiv.POnKeyDownAsync(keyEventArgs.Key, keyEventArgs.Modifiers);
                    if (Environment.CurrentManagedThreadId != 1)
                    {
                        Debugger.Break();
                    }

                    _builder.ShouldRebuild(GetContainingComponent(ActiveDiv));
                });
            }
        }

        if (args is RawTextInputEventArgs rawInputEventArgs)
        {
            if (ActiveDiv?.POnTextInput is not null)
            {
                ActiveDiv.POnTextInput(rawInputEventArgs.Text, rawInputEventArgs.Modifiers);
                objectsWhichReceivedACallback.Add(ActiveDiv);
            }
        }

        if (args is RawPointerEventArgs pointer)
        {
            var x = _windowManager.Scale((int)pointer.Position.X);
            var y = _windowManager.Scale((int)pointer.Position.Y);

            if (pointer.Type == RawPointerEventType.LeftButtonDown && _builder.Root is Div divRoot)
            {
                var div = HitTest(divRoot, x, y);

                if (div is null)
                    return;

                if (ActiveDiv?.POnInactive is not null)
                {
                    ActiveDiv.POnInactive();
                    objectsWhichReceivedACallback.Add(ActiveDiv);
                }

                if (ActiveDiv is not null)
                {
                    ActiveDiv.IsActive = false;
                }

                ActiveDiv = div;

                ActiveDiv.IsActive = true;

                if (div.POnActive is not null)
                {
                    div.POnActive();
                    objectsWhichReceivedACallback.Add(div);
                }

                if (div.POnClick is not null)
                {
                    div.POnClick();
                    objectsWhichReceivedACallback.Add(div);
                }

                if (div.POnClickAsync is not null)
                {
                    div.POnClickAsync();
                    objectsWhichReceivedACallback.Add(div);
                }
            }

            if (pointer.Type == RawPointerEventType.Move && _builder.Root is Div divRoot2)
            {
                if (HoveredDiv is not null)
                {
                    var res = HitTest(HoveredDiv, pointer.Position.X, pointer.Position.Y);

                    if (res is null)
                    {
                        HoveredDiv.IsHovered = false;
                        HoveredDiv = HitTest(divRoot2, pointer.Position.X, pointer.Position.Y);
                        if (HoveredDiv is not null)
                        {
                            HoveredDiv.IsHovered = true;
                            _windowManager.DoPaint(new Rect());
                        }
                    }
                    else
                    {
                        if (res != HoveredDiv)
                        {
                            HoveredDiv.IsHovered = false;
                            HoveredDiv = res;
                            HoveredDiv.IsHovered = true;
                            _windowManager.DoPaint(new Rect());
                        }
                    }
                }
                else
                {
                    HoveredDiv = HitTest(divRoot2, pointer.Position.X, pointer.Position.Y);
                    if (HoveredDiv is not null)
                    {
                        HoveredDiv.IsHovered = true;
                        _windowManager.DoPaint(new Rect());
                    }
                }
            }
        }


        foreach (var renderObject in objectsWhichReceivedACallback)
        {
            _builder.ShouldRebuild(GetContainingComponent(renderObject));
        }
    }

    private Component GetContainingComponent(RenderObject renderObject)
    {
        var current = renderObject;

        while (current is not CustomRenderObject)
        {
            current = current.Parent;

            if (current is null)
            {
                return _builder.RootComponent;
            }
        }

        return ((CustomRenderObject)current).Component;
    }

    private static Div? HitTest(Div div, double x, double y)
    {
        if (DivContainsPoint(div, x, y))
        {
            if (div.Children is null)
            {
                return div;
            }

            foreach (var child in div.Children)
            {
                var actualChild = child;

                if (child is CustomRenderObject customRenderObject)
                {
                    actualChild = customRenderObject.RenderObject;
                }

                if (actualChild is not Div divChild) continue;

                var childHit = HitTest(divChild, x, y);
                if (childHit is not null)
                    return childHit;
            }

            return div;
        }

        return null;
    }

    private static bool DivContainsPoint(Div div, double x, double y)
    {
        return div.PComputedX <= x && div.PComputedX + div.PComputedWidth >= x && div.PComputedY <= y &&
               div.PComputedY + div.PComputedHeight >= y;
    }
}