﻿using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using System;
using System.Diagnostics;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{
    public class StrykerVsTestHostLauncher : ITestHostLauncher
    {
        private readonly Action _callback;
        private readonly int? _activeMutation;

        public StrykerVsTestHostLauncher(Action callback, int? activeMutation)
        {
            _callback = callback;
            _activeMutation = activeMutation;
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
            processInfo.EnvironmentVariables["ActiveMutation"] = _activeMutation.ToString();

            var process = new Process { StartInfo = processInfo, EnableRaisingEvents = true };
            process.Start();

            if (process != null)
            {
                process.Exited += (sender, args) =>
                {
                    _callback();
                };

                return process.Id;
            }

            throw new Exception("Process in invalid state.");
        }

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo)
        {
            return LaunchTestHost(defaultTestHostStartInfo, CancellationToken.None);
        }
    }
}
