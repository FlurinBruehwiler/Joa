namespace JoaKit;

public class LayoutEngine
{
    public void ApplyLayoutCalculations(Div newRoot, Div oldRoot)
    {
        if (newRoot.LayoutHasChanged(oldRoot))
        {
            ComputeRenderObj(newRoot);
        }
    }

    private void ComputeRenderObj(RenderObject renderObject)
    {
        if (renderObject is Div div)
        {
            ComputedDiv(div);
        }
    }
    
    private void ComputedDiv(Div div)
    {
        if (div.Children is null)
            return;
        
        ComputedSize(div);
        ComputePosition(div);
        
        foreach (var child in div.Children)
        {
            ComputeRenderObj(child);    
        }
    }

    private void ComputedSize(Div div)
    {
        switch (div.PDir)
        {
            case Dir.Row or Dir.RowReverse:
                ComputeRowSize(div);
                break;
            case Dir.Column or Dir.ColumnReverse:
                ComputeColumnSize(div);
                break;
        }
    }
    
    private void ComputePosition(Div div)
    {
        switch (div.PmAlign)
        {
            case MAlign.FlexStart:
                RenderFlexStart(div);
                break;
            case MAlign.FlexEnd:
                RenderFlexEnd(div);
                break;
            case MAlign.Center:
                RenderCenter(div);
                break;
            case MAlign.SpaceBetween:
                RenderSpaceBetween(div);
                break;
            case MAlign.SpaceAround:
                RenderSpaceAround(div);
                break;
            case MAlign.SpaceEvenly:
                RenderSpaceEvenly(div);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private float GetCrossAxisOffset(Div div, RenderObject item)
    {
        return div.PxAlign switch
        {
            XAlign.FlexStart => 0,
            XAlign.FlexEnd => GetCrossAxisLength(div) - GetItemCrossAxisLength(div, item),
            XAlign.Center => GetCrossAxisLength(div) / 2 - GetItemCrossAxisLength(div, item) / 2,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private float GetMainAxisLength(Div div)
    {
        return div.PDir switch
        {
            Dir.Row or Dir.RowReverse => div.PComputedWidth - 2 * div.PPadding,
            Dir.Column or Dir.ColumnReverse => div.PComputedHeight - 2 * div.PPadding,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private float GetCrossAxisLength(Div div)
    {
        return div.PDir switch
        {
            Dir.Row or Dir.RowReverse => div.PComputedHeight - 2 * div.PPadding,
            Dir.Column or Dir.ColumnReverse => div.PComputedWidth - 2 * div.PPadding,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private float GetItemMainAxisLength(Div div, RenderObject item)
    {
        return div.PDir switch
        {
            Dir.Row or Dir.RowReverse => item.PComputedWidth,
            Dir.Column or Dir.ColumnReverse => item.PComputedHeight,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private float GetItemMainAxisFixedLength(Div div, RenderObject item)
    {
        return div.PDir switch
        {
            Dir.Row or Dir.RowReverse => item.PWidth.Kind == SizeKind.Percentage
                ? 0
                : item.PWidth.Value,
            Dir.Column or Dir.ColumnReverse => item.PHeight.Kind == SizeKind.Percentage
                ? 0
                : item.PHeight.Value,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private float GetItemCrossAxisLength(Div div, RenderObject item)
    {
        return div.PDir switch
        {
            Dir.Row or Dir.RowReverse => item.PComputedHeight,
            Dir.Column or Dir.ColumnReverse => item.PComputedWidth,
            _ => throw new ArgumentOutOfRangeException()
        };
    }


    private void ComputeColumnSize(Div div)
    {
        var remainingSize = RemainingMainAxisFixedSize(div);
        
        var totalPercentage = 0f;
        
        foreach (var child in div.Children)
        {
            if (child.PHeight.Kind == SizeKind.Percentage)
            {
                totalPercentage += child.PHeight.Value;
            }
        }
        
        float sizePerPercent;

        if (totalPercentage > 100)
        {
            sizePerPercent = remainingSize / totalPercentage;
        }
        else
        {
            sizePerPercent = remainingSize / 100;
        }

        foreach (var item in div.Children)
        {
            item.PComputedHeight = item.PHeight.Kind switch
            {
                SizeKind.Percentage => item.PHeight.Value * sizePerPercent,
                SizeKind.Pixel => item.PHeight.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
            item.PComputedWidth = item.PWidth.Kind switch
            {
                SizeKind.Pixel => item.PWidth.Value,
                SizeKind.Percentage => (float)((div.PComputedWidth - 2 * div.PPadding) * item.PWidth.Value * 0.01),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    private void ComputeRowSize(Div div)
    {
        var remainingSize = RemainingMainAxisFixedSize(div);
        var totalPercentage = 0f;

        foreach (var child in div.Children)
        {
            if (child.PWidth.Kind == SizeKind.Percentage)
            {
                totalPercentage += child.PWidth.Value;
            }
        }
        
        float sizePerPercent;

        if (totalPercentage > 100)
        {
            sizePerPercent = remainingSize / totalPercentage;
        }
        else
        {
            sizePerPercent = remainingSize / 100;
        }

        foreach (var item in div.Children)
        {
            item.PComputedWidth = item.PWidth.Kind switch
            {
                SizeKind.Percentage => item.PWidth.Value * sizePerPercent,
                SizeKind.Pixel => item.PWidth.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
            item.PComputedHeight = item.PHeight.Kind switch
            {
                SizeKind.Pixel => item.PHeight.Value,
                SizeKind.Percentage => (float)((div.PComputedHeight * item.PHeight.Value * 0.01) - (2 * div.PPadding)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    private float RemainingMainAxisFixedSize(Div div)
    {
        var childSum = 0f;
        
        foreach (var child in div.Children)
        {
            childSum += GetItemMainAxisFixedLength(div, child);
        }
        
        
        return GetMainAxisLength(div) - childSum - GetGapSize(div);
    }

    private float GetGapSize(Div div)
    {
        if (div.Children.Count <= 1)
            return 0;

        return (div.Children.Count - 1) * div.PGap;
    }

    private float RemainingMainAxisSize(Div div)
    {
        var sum = 0f;
        
        foreach (var child in div.Children)
        {
            sum += GetItemMainAxisLength(div, child);
        }
        
        return GetMainAxisLength(div) - sum;
    }

    private void RenderFlexStart(Div div)
    {
        var mainOffset = 0f;

        foreach (var child in div.Children)
        {
            DrawWithMainOffset(div, mainOffset, child);
            mainOffset += GetItemMainAxisLength(div, child) + div.PGap;
        }
    }

    private void RenderFlexEnd(Div div)
    {
        var mainOffset = RemainingMainAxisSize(div);

        foreach (var child in div.Children)
        {
            DrawWithMainOffset(div, mainOffset, child);
            mainOffset += GetItemMainAxisLength(div, child) + div.PGap;
        }
    }

    private void RenderCenter(Div div)
    {
        var mainOffset = RemainingMainAxisSize(div) / 2;

        foreach (var child in div.Children)
        {
            DrawWithMainOffset(div, mainOffset, child);
            mainOffset += GetItemMainAxisLength(div, child) + div.PGap;
        }
    }

    private void RenderSpaceBetween(Div div)
    {
        var totalRemaining = RemainingMainAxisSize(div);
        var space = totalRemaining / (div.Children.Count - 1);

        var mainOffset = 0f;

        foreach (var child in div.Children)
        {
            DrawWithMainOffset(div, mainOffset, child);
            mainOffset += GetItemMainAxisLength(div, child) + space + div.PGap;
        }
    }

    private void RenderSpaceAround(Div div)
    {
        var totalRemaining = RemainingMainAxisSize(div);
        var space = totalRemaining / div.Children.Count / 2;

        var mainOffset = 0f;

        foreach (var child in div.Children)
        {
            mainOffset += space;
            DrawWithMainOffset(div, mainOffset, child);
            mainOffset += GetItemMainAxisLength(div, child) + space + div.PGap;
        }
    }

    private void RenderSpaceEvenly(Div div)
    {
        var totalRemaining = RemainingMainAxisSize(div);
        var space = totalRemaining / (div.Children.Count + 1);

        var mainOffset = space;

        foreach (var child in div.Children)
        {
            DrawWithMainOffset(div, mainOffset, child);
            mainOffset += GetItemMainAxisLength(div, child) + space + div.PGap;
        }
    }

    private void DrawWithMainOffset(Div div, float mainOffset, RenderObject item)
    {
        switch (div.PDir)
        {
            case Dir.Row:
                item.PComputedX = mainOffset;
                item.PComputedY = GetCrossAxisOffset(div, item);
                break;
            case Dir.RowReverse:
                item.PComputedX = div.PComputedWidth - 2 * div.PPadding - mainOffset - item.PComputedWidth;
                item.PComputedY = GetCrossAxisOffset(div, item);
                break;
            case Dir.Column:
                item.PComputedX = GetCrossAxisOffset(div, item);
                item.PComputedY = mainOffset;
                break;
            case Dir.ColumnReverse:
                item.PComputedX = GetCrossAxisOffset(div, item);
                item.PComputedY = div.PComputedHeight - mainOffset - item.PComputedHeight;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        item.PComputedX += div.PComputedX + div.PPadding;
        item.PComputedY += div.PComputedY + div.PPadding;
    }
}