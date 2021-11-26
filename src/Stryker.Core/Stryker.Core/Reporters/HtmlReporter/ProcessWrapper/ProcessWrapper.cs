using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Stryker.Core.Reporters.HtmlReporter.ProcessWrapper
{
    public class ProcessWrapper : IProcessWrapper
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
