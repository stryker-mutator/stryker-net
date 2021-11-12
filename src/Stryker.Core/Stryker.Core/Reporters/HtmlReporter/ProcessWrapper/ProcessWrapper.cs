using System.Diagnostics;

namespace Stryker.Core.Reporters.HtmlReporter.ProcessWrapper
{
    public class ProcessWrapper : IProcessWrapper
    {
        public Process Start(string filePath)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            };

            return Process.Start(processInfo);
        }
    }
}
