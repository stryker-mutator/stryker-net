using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

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
                var wslPathProcessInfo = new ProcessStartInfo
                {
                    FileName = "wslpath",
                    Arguments = $"-wa {path}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,

                };

                var wslPathProcess = new Process { StartInfo = wslPathProcessInfo };
                if (wslPathProcess.Start())
                {
                    var windowsPath = wslPathProcess.StandardOutput.ReadToEnd();
                    var powershellCommand = $"Start-Process {windowsPath}";

                    processInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-NoProfile -Command {powershellCommand}",
                        UseShellExecute = true
                    };
                }
                else
                {
                    ApplicationLogging.LoggerFactory.CreateLogger<CrossPlatformBrowserOpener>().LogError("Failed to auto-open browser. Please report your runtime and OS info.");
                    return null;
                }
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
