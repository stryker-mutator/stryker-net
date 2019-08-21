using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.TestRunners.VsTest
{
    public interface IStrykerTestHostLauncher : ITestHostLauncher
    {
        bool WaitProcessExit();
    }

    public class StrykerVsTestHostLauncher : IStrykerTestHostLauncher
    {
        private readonly IDictionary<string, string> _envVars;
        private Process _currentProcess;
        private readonly object _lck = new object();
        private readonly int _id;
        private static ILogger Logger { get; }

        static StrykerVsTestHostLauncher()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerVsTestHostLauncher>();
        }

        public StrykerVsTestHostLauncher(IDictionary<string, string> envVars, int id)
        {
            _envVars = envVars;
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
            };
            foreach (var (key, value) in _envVars)
            {
                processInfo.EnvironmentVariables[key] = value;
            }
            _currentProcess = new Process {StartInfo = processInfo, EnableRaisingEvents = true};

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
                    Monitor.Wait(_lck, 5000);
                }
            }
            return true;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Logger.LogDebug($"{e.Data} (Runner {_id}: VsTest error)");
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Logger.LogDebug($"{e.Data} (Runner {_id}: VsTest output)");
            }
        }

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo)
        {
            return LaunchTestHost(defaultTestHostStartInfo, CancellationToken.None);
        }
    }
}
