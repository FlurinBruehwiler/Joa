using System.Drawing;
using JoaPluginsPackage.Plugin;

namespace ApplicationSearch;

public class ColorConverter : IStrictPlugin
{
    public bool Validator(string searchString)
    {
        return true;
    }

    public List<ICommand> GetResults(string searchString)
    {
        throw new NotImplementedException();
    }

    public void Execute(ICommand command)
    {
        throw new NotImplementedException();
    }
    
    private string HexToRgb(string hexColor)
    {
        var color = ColorTranslator.FromHtml(hexColor);
        int r = Convert.ToInt16(color.R);
        int g = Convert.ToInt16(color.G);
        int b = Convert.ToInt16(color.B);
        return $"rgba({r}, {g}, {b});";
    }

    private string HexToRrbA(string hexColor)
    {
        var rgbColor = HexToRgb(hexColor);
        return rgbColor.Insert(rgbColor.Length - 1, ", 1");
    }

    public string RgbToHex(int r, int g, int b)
    {
        var myColor = Color.FromArgb(r, g, b);
        return myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2");
    }
}