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
            var processInfo = new ProcessStartInfo(
                                  defaultTestHostStartInfo.FileName,
                                  defaultTestHostStartInfo.Arguments)
            {
                WorkingDirectory = defaultTestHostStartInfo.WorkingDirectory
            };
            foreach (var (key, value) in _envVars)
            {
                processInfo.EnvironmentVariables[key] = value;
            }

            var process = new Process { StartInfo = processInfo, EnableRaisingEvents = true };
            process.Start();

            process.Exited += (sender, args) =>
            {
                _callback();
            };

            return process.Id;
        }

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo)
        {
            return LaunchTestHost(defaultTestHostStartInfo, CancellationToken.None);
        }
    }
}
