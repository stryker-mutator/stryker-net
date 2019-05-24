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
    public class StrykerVsTestHostLauncher : ITestHostLauncher
    {
        private readonly Action _callback;
        private readonly Dictionary<string, string> _envVars;
        private Process _currentProcess;
        private readonly object lck = new object();
        private static ILogger Logger { get; }

        static StrykerVsTestHostLauncher()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerVsTestHostLauncher>();
        }

        public StrykerVsTestHostLauncher(Action callback, Dictionary<string, string> envVars)
        {
            _callback = callback;
            _envVars = envVars;
        }

        public bool IsDebug => false;

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo, CancellationToken cancellationToken)
        {
            var redirect = true;
            var processInfo = new ProcessStartInfo(defaultTestHostStartInfo.FileName, defaultTestHostStartInfo.Arguments)
            {
                WorkingDirectory = defaultTestHostStartInfo.WorkingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = redirect,
                RedirectStandardError = redirect,
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
                Logger.LogError($"Failed to start process {processInfo.Arguments}.");
            }
            // Asynchronously read the standard output of the spawned process.
            // This raises OutputDataReceived events for each line of output.
            if (redirect)
            {
                _currentProcess.BeginOutputReadLine();
                _currentProcess.BeginErrorReadLine();
            }

            return _currentProcess.Id;
        }

        private void CurrentProcess_Exited(object sender, EventArgs e)
        {
            lock (lck)
            {
                Monitor.Pulse(lck);
            }
            _callback();
        }

        public bool WaitProcessExit()
        {
            if (_currentProcess == null)
            {
                Logger.LogInformation("VsTest returned cached results.");
                return false;
            }
            while (!_currentProcess.HasExited)
            {
                lock (lck)
                {
                    Monitor.Wait(lck, 5000);
                }
            }
            return true;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Logger.LogInformation($"{Environment.NewLine}{e.Data} (VsTest error)");
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Logger.LogDebug($"{e.Data} (VsTest output)");
            }
        }

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo)
        {
            return LaunchTestHost(defaultTestHostStartInfo, CancellationToken.None);
        }
    }
}
