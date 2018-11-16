using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Stryker.Core.Testing
{
    /// <summary>
    /// Used for mocking System.Process 
    /// </summary>
    public interface IProcessExecutor
    {
        /// <summary>
        /// Starts an process and returns the result when done. Takes an environment variable for active mutation
        /// </summary>
        /// <param name="path">The path the process will use as base path</param>
        /// <param name="application">example: dotnet</param>
        /// <param name="arguments">example: --no-build</param>
        /// <param name="activeMutationId">this value will be used to set an environment variable for the process</param>
        /// <returns>ProcessResult</returns>
        ProcessResult Start(string path, string application, string arguments, IEnumerable<KeyValuePair<string, string>> environmentVariables = null, int timeoutMS = 0);
    }
    
    [ExcludeFromCodeCoverage]
    public class ProcessExecutor : IProcessExecutor
    {
        // when redirected, the output from the process will be kept in memory and not displayed to the console directly
        private bool RedirectOutput { get; }

        public ProcessExecutor(bool redirectOutput = true)
        {
            RedirectOutput = redirectOutput;
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
                RedirectStandardOutput = RedirectOutput,
                RedirectStandardError = RedirectOutput
            };

            foreach (var environmentVariable in environmentVariables ?? Enumerable.Empty<KeyValuePair<string, string>>())
            {
                info.EnvironmentVariables[environmentVariable.Key] = environmentVariable.Value;
            }

            return RunProcess(info, timeoutMS);
        }

        /// <summary>
        /// Starts a process with the given info. 
        /// Checks for timeout after <paramref name="timeoutMS"/> milliseconds if the process is still running. 
        /// </summary>
        /// <param name="info">The start info for the process</param>
        /// <param name="timeoutMS">The milliseconds to check for a timeout</param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        private ProcessResult RunProcess(ProcessStartInfo info, int timeoutMS)
        {
            using (var process = new ProcessWrapper(info))
            {
                var timeoutValue = timeoutMS == 0 ? -1 : timeoutMS;
                if (!process.WaitForExit(timeoutValue))
                {
                    throw new OperationCanceledException("The process was terminated due to long runtime");
                }

                return new ProcessResult()
                {
                    ExitCode = process.ExitCode,
                    Output = process.Output
                };
            }
        }

        private sealed class ProcessWrapper: IDisposable
        {
            private readonly Process process;
            private readonly StringBuilder output = new StringBuilder();
            private readonly StringBuilder error = new StringBuilder();

            public int ExitCode => process.ExitCode;
            public string Output => output.ToString();

            public ProcessWrapper(ProcessStartInfo info)
            {
                process = Process.Start(info);
                process.OutputDataReceived += Process_OutputDataReceived;
                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }

            public bool WaitForExit(int timeout = -1)
            {
                var totalWait = 0;
                var slice = timeout == -1 ? timeout : timeout / 20;
                do
                {
                    if (process.WaitForExit(slice))
                    {
                        return true;
                    }

                    totalWait += slice;
                } while (timeout==-1 || totalWait < timeout);

                process.KillTree(TimeSpan.FromSeconds(60));
                return false;
            }

            private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                {
                    output.AppendLine(e.Data);
                }
            }

            private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                {
                    output.AppendLine(e.Data);
                }
            }

            public void Dispose()
            {
                process.OutputDataReceived -= Process_OutputDataReceived;
                process.ErrorDataReceived -= Process_ErrorDataReceived;
                process.Dispose();
            }
        }
    }
}
