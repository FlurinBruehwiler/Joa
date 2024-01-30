using FlurinBruehwiler.Helpers;
using JoaLauncher.Api;
using JoaLauncher.Api.Injectables;
using JoaLauncher.Api.Plugin;
using JoaLauncher.Api.Providers;
using org.matheval;

namespace Calculator;

public class Calculator : IPlugin, IProvider
{
    private readonly IClipboardHelper _clipboardHelper;
    private readonly Expression _expression;

    public Calculator(IClipboardHelper clipboardHelper)
    {
        _clipboardHelper = clipboardHelper;
        _expression = new Expression();
    }
    
    public void ConfigurePlugin(IPluginBuilder builder)
    {
        builder.AddGlobalProvider<Calculator>(s =>
        {
            try
            {
                _expression.SetFomular(s);
                _expression.Eval<double>();
                return true;
            }
            catch
            {
                return false;
            }
        });
    }

    public List<SearchResult> GetSearchResults(string searchString)
    {
        _expression.SetFomular(searchString);
        var res = _expression.Eval<double>();

        return new List<SearchResult>
        {
            new CalculatorSearchResult
            {
                Title = $"= {res}",
                Description = "",
                Icon = "",
                ClipboardHelper = _clipboardHelper,
                Value = res
            }
        };
    }
}
