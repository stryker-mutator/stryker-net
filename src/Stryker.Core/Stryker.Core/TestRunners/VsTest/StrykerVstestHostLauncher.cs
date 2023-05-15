using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using Stryker.Core.Logging;
using System.Diagnostics;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{

    public interface IStrykerTestHostLauncher : ITestHostLauncher
    {
         bool IsProcessCreated { get; }
         int ErrorCode { get; }
    }

    public class StrykerVsTestHostLauncher : IStrykerTestHostLauncher
    {
        private readonly string _id;
        private readonly bool _devMode;
        private Process _currentProcess;
        public int ErrorCode { get; private set; }

        private static ILogger Logger { get; }

        static StrykerVsTestHostLauncher() => Logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerVsTestHostLauncher>();

        public StrykerVsTestHostLauncher(string id, bool devMode)
        {
            _id = id;
            _devMode = devMode;
        }

        public bool IsDebug => false;

        public bool IsProcessCreated => _currentProcess != null;

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo) =>
            LaunchTestHost(defaultTestHostStartInfo, CancellationToken.None);

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
            _currentProcess = new Process { StartInfo = processInfo, EnableRaisingEvents = true };

            if (_devMode)
            {
                _currentProcess.OutputDataReceived += Process_OutputDataReceived;
                _currentProcess.ErrorDataReceived += Process_ErrorDataReceived;
            }
            if (!_currentProcess.Start())
            {
                Logger.LogError($"Runner {_id}: Failed to start process {processInfo.Arguments}.");
            }

            _currentProcess.BeginOutputReadLine();
            _currentProcess.BeginErrorReadLine();
            _currentProcess.Exited += ProcessExited;

            return _currentProcess.Id;
        }

        private void ProcessExited(object sender, System.EventArgs e)
        {
            var process = (Process) sender;
            process.WaitForExit();
            ErrorCode = process.ExitCode;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Logger.LogDebug($"{_id}: {e.Data} (VsTest error)");
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Logger.LogTrace($"{_id}: {e.Data} (VsTest output)");
            }
        }
    }
}
