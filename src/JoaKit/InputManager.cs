using System.Diagnostics;
using Modern.WindowKit.Input;
using Modern.WindowKit.Input.Raw;
using Modern.WindowKit.Threading;

namespace JoaKit;

public class InputManager
{
    private readonly Builder _builder;
    private readonly WindowManager _windowManager;
    public Div? ActiveDiv { get; set; }

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
                        //NotGood
                        Debugger.Break();
                    }
                    await ActiveDiv.POnKeyDownAsync(keyEventArgs.Key, keyEventArgs.Modifiers);
                    if (Environment.CurrentManagedThreadId != 1)
                    {
                        //NotGood
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

                ActiveDiv = div;

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
        }

        return ((CustomRenderObject)current).Component;
    }

    private static Div? HitTest(Div div, int x, int y)
    {
        if (div.PComputedX <= x && div.PComputedX + div.PComputedWidth >= x && div.PComputedY <= y && div.PComputedY + div.PComputedHeight >= y)
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
}