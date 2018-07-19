using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stryker.Core.Testing
{
    [ExcludeFromCodeCoverage]
    public class ProcessExecutor : IProcessExecutor
    {
        // when redirected, the output from the process will be kept in memory and not displayed to the console directly
        private bool _redirectOutput { get; set; }

        public ProcessExecutor(bool redirectOutput = true)
        {
            _redirectOutput = redirectOutput;
        }

        public ProcessResult Start(
            string path,
            string application,
            string arguments,
            IEnumerable<KeyValuePair<string, string>> environmentVariables = null,
            int timeoutMS = 0)
        {
            var info = new ProcessStartInfo(application, arguments)
            {
                UseShellExecute = false,
                WorkingDirectory = path,
                RedirectStandardOutput = _redirectOutput,
                RedirectStandardError = _redirectOutput
            };

            foreach (var environmentVariable in environmentVariables ?? Enumerable.Empty<KeyValuePair<string, string>>())
            {
                info.EnvironmentVariables[environmentVariable.Key] = environmentVariable.Value;
            }

            return RunProcess(info, timeoutMS);
        }

        /// <summary>
        /// Starts a process with the given info. 
        /// Checks for timeout after <paramref name="timeoutMS"/> miliseconds if the process is still running. 
        /// </summary>
        /// <param name="info">The start info for the process</param>
        /// <param name="timeoutMS">The miliseconds to check for a timeout</param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        private ProcessResult RunProcess(ProcessStartInfo info, int timeoutMS)
        {
            using (var process = Process.Start(info))
            {
                string output = "";
                var processDoneHandle = new ManualResetEvent(false);

                Task.Run(() =>
                {
                    if (_redirectOutput)
                    {
                        output = process.StandardOutput.ReadToEnd();
                    }
                    process.WaitForExit();
                    // When the process exited, trigger the processDoneEvent
                    processDoneHandle.Set();
                });


                // This handle will wait till the process has exited, or the timeoutMS has passed
                int timeoutValue = timeoutMS == 0 ? -1 : timeoutMS;
                var processDone = processDoneHandle.WaitOne(timeoutValue);

                if (!processDone)
                {
                    // The process is still running. Kill the process and all it's child processes.
                    process.KillTree(TimeSpan.FromSeconds(60));
                    throw new OperationCanceledException("The process was terminated due to long runtime");
                }
                return new ProcessResult()
                {
                    ExitCode = process.ExitCode,
                    Output = output
                };
            }
        }
    }
}
