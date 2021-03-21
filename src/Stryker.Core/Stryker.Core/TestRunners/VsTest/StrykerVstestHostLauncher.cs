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
        bool WaitProcessExit();
    }

    public class StrykerVsTestHostLauncher : IStrykerTestHostLauncher
    {
        private Process _currentProcess;
        private readonly object _lck = new object();
        private readonly int _id;

        private static ILogger Logger { get; }

        static StrykerVsTestHostLauncher()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerVsTestHostLauncher>();
        }

        public StrykerVsTestHostLauncher(int id)
        {
            _id = id;
        }

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
                },
            };
            _currentProcess = new Process { StartInfo = processInfo, EnableRaisingEvents = true };

            _currentProcess.Exited += CurrentProcess_Exited;

            _currentProcess.OutputDataReceived += Process_OutputDataReceived;
            _currentProcess.ErrorDataReceived += Process_ErrorDataReceived;

            if (!_currentProcess.Start())
            {
                Logger.LogError($"Runner {_id}: Failed to start process {processInfo.Arguments}.");
            }

            _currentProcess.BeginOutputReadLine();
            _currentProcess.BeginErrorReadLine();

            return _currentProcess.Id;
        }

        private void CurrentProcess_Exited(object sender, EventArgs e)
        {
            lock (_lck)
            {
                Monitor.Pulse(_lck);
            }
        }

        public bool WaitProcessExit()
        {
            if (_currentProcess == null)
            {
                return false;
            }

            while (!_currentProcess.HasExited)
            {
                lock (_lck)
                {
                    Monitor.Wait(_lck, 500);
                }
            }
            return true;
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

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo)
        {
            return LaunchTestHost(defaultTestHostStartInfo, CancellationToken.None);
        }
    }
}
