using System.Drawing;
using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Plugin;

namespace ColorConverter;

[Plugin("Color Converter", "", "", "", "")]
public class ColorConverter : IStrictSearchPlugin
{
    public bool Validator(string searchString)
    {
        return false;
    }

     public List<ISearchResult> GetStrictSearchResults(string searchString)
     {
        return new List<ISearchResult>();
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