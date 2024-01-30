namespace JoaLauncher.Api.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class RangeAttribute : Attribute
{
    public int From { get; set; }
    public int To { get; set; }

    public RangeAttribute(int from, int to)
    {
        From = from;
        To = to;
    }
}