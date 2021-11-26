using System.Diagnostics;

namespace Stryker.Core.Reporters.HtmlReporter.ProcessWrapper
{
    public class WebbrowserOpener : IWebbrowserOpener
    {
        public Process Open(string path)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            };

            return Process.Start(processInfo);
        }
    }
}
