using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{
    public class StrykerVsTestHostLauncher : ITestHostLauncher
    {
        private readonly Action _callback;
        private readonly Dictionary<string, string> _envVars;

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
                RedirectStandardError = redirect
            };
            foreach (var (key, value) in _envVars)
            {
                processInfo.EnvironmentVariables[key] = value;
            }
            var process = new Process {StartInfo = processInfo, EnableRaisingEvents = true};

            process.Exited += (sender, args) =>
            {
                _callback();
            };
            process.Start();
            // Asynchronously read the standard output of the spawned process.
            // This raises OutputDataReceived events for each line of output.
            if (redirect)
            {
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }

            return process.Id;
        }

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo)
        {
            return LaunchTestHost(defaultTestHostStartInfo, CancellationToken.None);
        }
    }
}
