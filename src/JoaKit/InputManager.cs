using Modern.WindowKit;
using Modern.WindowKit.Input.Raw;
using Modern.WindowKit.Threading;

namespace JoaKit;

public class InputManager
{
    private readonly Renderer _renderer;
    private readonly WindowManager _windowManager;
    private Div? _activeDiv;

    public InputManager(Renderer renderer, WindowManager windowManager)
    {
        _renderer = renderer;
        _windowManager = windowManager;
    }

    public void Input(RawInputEventArgs args)
    {
        var callbackWasCalled = false;

        if (args is RawKeyEventArgs { Type: RawKeyEventType.KeyDown } keyEventArgs)
        {
            if (_activeDiv?.POnKeyDown is not null)
            {
                _activeDiv.POnKeyDown(keyEventArgs.Key, keyEventArgs.Modifiers);
                callbackWasCalled = true;
            }

            if (_activeDiv?.POnKeyDownAsync is not null)
            {
                Task.Run(async () =>
                {
                    await _activeDiv.POnKeyDownAsync(keyEventArgs.Key, keyEventArgs.Modifiers);
                    _renderer.Build(_windowManager.RootComponent);
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        _windowManager.DoPaint(new Rect());
                    });
                });
            }
        }
        if (args is RawTextInputEventArgs rawInputEventArgs)
        {
            if (_activeDiv?.POnTextInput is not null)
            {
                _activeDiv.POnTextInput(rawInputEventArgs.Text, rawInputEventArgs.Modifiers);
                callbackWasCalled = true;
            }
        }
        if (args is RawPointerEventArgs pointer)
        {
            var x = _windowManager.Scale((int)pointer.Position.X);
            var y = _windowManager.Scale((int)pointer.Position.Y);

            if (pointer.Type == RawPointerEventType.LeftButtonDown && _renderer.Root is Div divRoot)
            {
                var div = HitTest(divRoot, x, y);

                if (div is null)
                    return;

                if (_activeDiv?.POnInactive is not null)
                {
                    _activeDiv.POnInactive();
                    callbackWasCalled = true;
                }

                _activeDiv = div;

                if (div.POnActive is not null)
                {
                    div.POnActive();
                    callbackWasCalled = true;
                }

                if (div.POnClick is not null)
                {
                    div.POnClick();
                    callbackWasCalled = true;
                }

                if (div.POnClickAsync is not null)
                {
                    div.POnClickAsync();
                    callbackWasCalled = true;
                }
            }
        }

        if (callbackWasCalled)
        {
            _renderer.Build(_windowManager.RootComponent);
            _windowManager.DoPaint(new Rect());
        }
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