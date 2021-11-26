using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Stryker.Core.Reporters.HtmlReporter.ProcessWrapper
{
    [ExcludeFromCodeCoverage]
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
