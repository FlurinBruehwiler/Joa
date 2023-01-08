using JoaLauncher.Api;

namespace Joa;
public class Options<T> : IOptions<T> where T : class
{
    public Options(T value)
    {
        Value = value;
    }

    public T Value { get; set; }
}

