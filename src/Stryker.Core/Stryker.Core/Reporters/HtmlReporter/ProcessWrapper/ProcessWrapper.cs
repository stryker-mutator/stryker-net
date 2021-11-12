using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Stryker.Core.Reporters.HtmlReporter.ProcessWrapper
{
    public class ProcessWrapper : IProcessWrapper
    {
        public Process StartFile(string filePath)
        {
            var processInfo = new ProcessStartInfo
            {
                
                FileName = filePath,
                UseShellExecute = true
            };

            return Process.Start(processInfo);
        }

        public Process StartUrl(string url)
        {
            try
            {
                return Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    return Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
