using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Web;
using JoaLauncher.Api.Injectables;

namespace JoaInterface.Injectables;

public class BrowserHelper : IBrowserHelper
{
    public void OpenWebsite(string url)
    {
        url = HttpUtility.UrlEncode(url);
        
        try
        {
            Process.Start(url);
        }
        catch
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}