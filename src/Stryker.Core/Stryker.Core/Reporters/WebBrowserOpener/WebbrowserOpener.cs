using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Stryker.Core.Reporters.WebBrowserOpener
{
    [ExcludeFromCodeCoverage]
    public class CrossPlatformBrowserOpener : IWebbrowserOpener
    {
        private static bool IsWsl => RuntimeInformation.OSDescription.Contains("linux", StringComparison.InvariantCultureIgnoreCase) &&
            RuntimeInformation.OSDescription.Contains("microsoft", StringComparison.InvariantCultureIgnoreCase);

        public Process Open(string path)
        {
            ProcessStartInfo processInfo;
            if (IsWsl)
            {
                var windowsPath = Process.Start("wslpath", $"-wa {path}").StandardOutput.ReadToEnd();
                var powershellCommand = $"Start-Process {windowsPath}";
                processInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -Command {{{powershellCommand}}}",
                    UseShellExecute = true
                };
            }
            else
            {
                processInfo = new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                };
            }

            return Process.Start(processInfo);
        }
    }
}
