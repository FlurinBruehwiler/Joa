namespace JoaKit;

public enum MAlign
{
    FlexStart,
    FlexEnd,
    Center,
    SpaceBetween,
    SpaceAround,
    SpaceEvenly
}

public enum Dir
{
    Row,
    RowReverse,
    Column,
    ColumnReverse
}

public enum XAlign
{
    FlexStart,
    FlexEnd,
    Center
}

public record JoaKitSize(int Value, SizeKind SizeKind);
