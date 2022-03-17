using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using Stryker.Core.Logging;
using System;
using System.Diagnostics;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{
    public interface IStrykerTestHostLauncher : ITestHostLauncher
    {
        bool WaitProcessExit(DateTime dateTime);
    }

    public class StrykerVsTestHostLauncher : ITestHostLauncher
    {
        private readonly int _id;

        private static ILogger Logger { get; }

        static StrykerVsTestHostLauncher() => Logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerVsTestHostLauncher>();

        public StrykerVsTestHostLauncher(int id) => _id = id;

        public bool IsDebug => false;

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo, CancellationToken cancellationToken)
        {
            var processInfo = new ProcessStartInfo(defaultTestHostStartInfo.FileName, defaultTestHostStartInfo.Arguments)
            {
                WorkingDirectory = defaultTestHostStartInfo.WorkingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                EnvironmentVariables =
                {
                    // Disable DiffEngine so that approval tests frameworks such as https://github.com/VerifyTests/Verify
                    // or https://github.com/approvals/ApprovalTests.Net (which both use DiffEngine under the hood)
                    // don't launch a diffing tool GUI on each failed test.
                    // See https://github.com/VerifyTests/DiffEngine/blob/6.6.1/src/DiffEngine/DisabledChecker.cs#L8
                    ["DiffEngine_Disabled"] = "true",
                    // Disable copying the command to accept the received version to the clipboard when using Verify
                    // See https://github.com/VerifyTests/Verify/blob/main/docs/clipboard.md
                    ["Verify_DisableClipboard"] = "true",
                },
            };
            var currentProcess = new Process { StartInfo = processInfo, EnableRaisingEvents = false };

            currentProcess.OutputDataReceived += Process_OutputDataReceived;
            currentProcess.ErrorDataReceived += Process_ErrorDataReceived;

            if (!currentProcess.Start())
            {
                Logger.LogDebug($"Runner {_id}: Failed to start process {processInfo.Arguments}.");
            }

            currentProcess.BeginOutputReadLine();
            currentProcess.BeginErrorReadLine();

            return currentProcess.Id;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Logger.LogTrace($"Runner {_id}: {e.Data} (VsTest error)");
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Logger.LogTrace($"Runner {_id}: {e.Data} (VsTest output)");
            }
        }

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo) => LaunchTestHost(defaultTestHostStartInfo, CancellationToken.None);
    }
}
