using System.Globalization;
using FlurinBruehwiler.Helpers;
using JoaLauncher.Api;
using JoaLauncher.Api.Injectables;

namespace Calculator;

public class CalculatorSearchResult : JoaLauncher.Api.SearchResult
{
    public required double Value { get; init; }
    public required IClipboardHelper ClipboardHelper { get; init; }
    
    public override void Execute(IExecutionContext executionContext)
    {
        ClipboardHelper.Write(Value.ToString(CultureInfo.InvariantCulture));
    }
}
