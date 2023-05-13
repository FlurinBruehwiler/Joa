using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Modern.WindowKit.Input;
using SkiaSharp;

namespace JoaKit;

public class Div : RenderObject, IEnumerable<RenderObject>
{
    public Div([CallerLineNumber] int lineNumer = -1, [CallerFilePath] string filePath = "")
    {
        PLineNumber = lineNumer;
        PFilePath = filePath;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<RenderObject>? Children { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ColorDefinition? PColor { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ColorDefinition? PHoverColor { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ColorDefinition? PBorderColor { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public int PPadding { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public int PGap { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public int PRadius { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public int PBorderWidth { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Dir PDir { get; set; } = JoaKit.Dir.Vertical;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public MAlign PmAlign { get; set; } = JoaKit.MAlign.FlexStart;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public XAlign PxAlign { get; set; } = JoaKit.XAlign.FlexStart;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Action? POnClick { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Action? POnActive { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Action? POnInactive { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Action<Key, RawInputModifiers>? POnKeyDown { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Func<Key, RawInputModifiers, Task>? POnKeyDownAsync { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Action<string, RawInputModifiers>? POnTextInput { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool PAutoFocus { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsHovered { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsActive { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Func<Task>? POnClickAsync { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool LayoutHasChanged(Div oldDiv)
    {
        // if (PWidth != oldDiv.PWidth)
        //     return true;
        //
        // if (PHeight != oldDiv.PHeight)
        //     return true;

        if ((oldDiv.Children?.Count ?? 0) != (Children?.Count ?? 0))
            return true;

        if (PPadding != oldDiv.PPadding)
            return true;

        if (PGap != oldDiv.PGap)
            return true;

        if (PDir != oldDiv.PDir)
            return true;

        if (PmAlign != oldDiv.PmAlign)
            return true;

        if (PxAlign != oldDiv.PxAlign)
            return true;

        if (Children is not null && oldDiv.Children is not null)
        {
            for (var i = Children.Count - 1; i >= 0; i--)
            {
                // if (Children[i].LayoutHasChanged(oldDiv.Children[i]))
                return true;
            }
        }

        //Copy layout calculations
        PComputedWidth = oldDiv.PComputedWidth;
        PComputedHeight = oldDiv.PComputedHeight;
        PComputedX = oldDiv.PComputedX;
        PComputedY = oldDiv.PComputedY;

        return false;
    }

    private ColorDefinition? GetColor()
    {
        if (IsHovered && PHoverColor is not null)
        {
            return PHoverColor;
        }

        return PColor;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void Render(SKCanvas canvas, RenderContext renderContext)
    {
        if (GetColor() is not null)
        {
            if (PBorderWidth != 0)
            {
                if (PRadius != 0)
                {
                    float borderRadius = PRadius + PBorderWidth;

                    canvas.DrawRoundRect(PComputedX - PBorderWidth,
                        PComputedY - PBorderWidth,
                        PComputedWidth + 2 * PBorderWidth,
                        PComputedHeight + 2 * PBorderWidth,
                        borderRadius,
                        borderRadius,
                        GetColor(PBorderColor ?? GetColor().Value));
                    canvas.DrawRoundRect(PComputedX,
                        PComputedY,
                        PComputedWidth,
                        PComputedHeight,
                        PRadius,
                        PRadius,
                        GetColor(GetColor().Value));
                }
                else
                {
                    canvas.DrawRect(PComputedX - PBorderWidth, PComputedY - PBorderWidth,
                        PComputedWidth + 2 * PBorderWidth, PComputedHeight + 2 * PBorderWidth,
                        GetColor(PBorderColor ?? GetColor().Value));
                    canvas.DrawRect(PComputedX, PComputedY, PComputedWidth, PComputedHeight,
                        GetColor(GetColor().Value));
                }
            }
            else
            {
                if (PRadius != 0)
                {
                    canvas.DrawRoundRect(PComputedX, PComputedY, PComputedWidth, PComputedHeight, PRadius, PRadius,
                        GetColor(GetColor().Value));
                }
                else
                {
                    canvas.DrawRect(PComputedX, PComputedY, PComputedWidth, PComputedHeight,
                        GetColor(GetColor().Value));
                }
            }
        }


        if (Children is not null)
        {
            foreach (var renderObject in Children)
            {
                renderObject.Render(canvas, renderContext);
            }
        }
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

    public Div Items(IEnumerable<RenderObject> children)
    {
        Children ??= new List<RenderObject>();
        Children.AddRange(children);
        return this;
    }

    public RenderObject Add(RenderObject child)
    {
        Children ??= new List<RenderObject>();
        Children.Add(child);
        return this;
    }

    public Div Width(float width, SizeKind sizeKind = SizeKind.Pixel)
    {
        PWidth = new SizeDefinition(width, sizeKind);
        return this;
    }

    public Div Height(float height, SizeKind sizeKind = SizeKind.Pixel)
    {
        PHeight = new SizeDefinition(height, sizeKind);
        return this;
    }

    public Div Color(float red, float green, float blue, float transparency = 255)
    {
        PColor = new ColorDefinition(red, green, blue, transparency);
        return this;
    }

    public Div Color(ColorDefinition color)
    {
        PColor = color;
        return this;
    }

    public Div HoverColor(float red, float green, float blue, float transparency = 255)
    {
        PHoverColor = new ColorDefinition(red, green, blue, transparency);
        return this;
    }

    public Div HoverColor(ColorDefinition color)
    {
        PHoverColor = color;
        return this;
    }

    public Div Padding(int padding)
    {
        PPadding = padding;
        return this;
    }

    public Div Gap(int gap)
    {
        PGap = gap;
        return this;
    }

    public Div Radius(int radius)
    {
        PRadius = radius;
        return this;
    }

    public Div BorderWidth(int borderWidth)
    {
        PBorderWidth = borderWidth;
        return this;
    }

    public Div BorderColor(ColorDefinition color)
    {
        PBorderColor = color;
        return this;
    }

    public Div BorderColor(float red, float green, float blue, float transparency = 255)
    {
        PBorderColor = new ColorDefinition(red, green, blue, transparency);
        return this;
    }

    public Div Dir(Dir dir)
    {
        PDir = dir;
        return this;
    }

    public Div MAlign(MAlign mAlign)
    {
        PmAlign = mAlign;
        return this;
    }

    public Div XAlign(XAlign xAlign)
    {
        PxAlign = xAlign;
        return this;
    }

    public Div OnClick(Action onClick)
    {
        POnClick = onClick;
        return this;
    }

    public Div OnClick(Func<Task> onClick)
    {
        POnClickAsync = onClick;
        return this;
    }

    public Div OnActive(Action onActive)
    {
        POnActive = onActive;
        return this;
    }

    public Div OnInactive(Action onInactive)
    {
        POnInactive = onInactive;
        return this;
    }

    public Div OnKeyDown(Action<Key> onKeyDown)
    {
        POnKeyDown = (key, _) => onKeyDown(key);
        return this;
    }

    public Div OnKeyDown(Action<Key, RawInputModifiers> onKeyDown)
    {
        POnKeyDown = onKeyDown;
        return this;
    }

    public Div OnKeyDown(Func<Key, Task> onKeyDown)
    {
        POnKeyDownAsync = (key, _) => onKeyDown(key);
        return this;
    }

    public Div OnKeyDown(Func<Key, RawInputModifiers, Task> onKeyDown)
    {
        POnKeyDownAsync = onKeyDown;
        return this;
    }

    public Div OnTextInput(Action<string> onTextInput)
    {
        POnTextInput = (s, _) => onTextInput(s);
        return this;
    }

    public Div OnTextInput(Action<string, RawInputModifiers> onTextInput)
    {
        POnTextInput = onTextInput;
        return this;
    }

    public Div AutoFocus(bool autoFocus = true)
    {
        PAutoFocus = autoFocus;
        return this;
    }

    public Div Key(string key)
    {
        PKey = key;
        return this;
    }

    public IEnumerator<RenderObject> GetEnumerator()
    {
        return Children?.GetEnumerator() ?? Enumerable.Empty<RenderObject>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}