using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Stryker.Core.Testing;

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
    /// <param name="environmentVariables">Environment variables (and their values)</param>
    /// <param name="timeoutMs">time allotted to the process for execution (0 or-1 for no limit)</param>
    /// <returns>ProcessResult</returns>
    ProcessResult Start(string path, string application, string arguments, IEnumerable<KeyValuePair<string, string>> environmentVariables = null, int timeoutMs = 0);

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
        int timeoutMs = 0)
    {
        var info = new ProcessStartInfo(application, arguments)
        {
            UseShellExecute = false,
            WorkingDirectory = Path.GetDirectoryName(FilePathUtils.NormalizePathSeparators(path)),
            RedirectStandardOutput = RedirectOutput,
            RedirectStandardError = RedirectOutput
        };

        foreach (var (key, value) in environmentVariables ?? Enumerable.Empty<KeyValuePair<string, string>>())
        {
            info.EnvironmentVariables[key] = value;
        }

        return RunProcess(info, timeoutMs);
    }

    /// <summary>
    /// Starts a process with the given info. 
    /// Checks for timeout after <paramref name="timeoutMs"/> milliseconds if the process is still running. 
    /// </summary>
    /// <param name="info">The start info for the process</param>
    /// <param name="timeoutMs">The milliseconds to check for a timeout</param>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns></returns>
    private ProcessResult RunProcess(ProcessStartInfo info, int timeoutMs)
    {
        var process = new ProcessWrapper(info, RedirectOutput);
        using (process)
        {
            var timeoutValue = timeoutMs == 0 ? -1 : timeoutMs;
            if (!process.WaitForExit(timeoutValue))
            {
                throw new OperationCanceledException("The process was terminated due to long runtime");
            }
            return new ProcessResult()
            {
                ExitCode = process.ExitCode,
                Output = process.Output,
                Error = process.Error
            };
        }
    }

    private sealed class ProcessWrapper : IDisposable
    {
        private static readonly TimeSpan KillTimeOut = TimeSpan.FromSeconds(60);

        private readonly Process _process;
        private readonly StringBuilder _output = new StringBuilder();
        private readonly StringBuilder _error = new StringBuilder();

        public int ExitCode => _process.ExitCode;
        public string Output => _output.ToString();
        public string Error => _error.ToString();

        public ProcessWrapper(ProcessStartInfo info, bool redirectOutput)
        {
            _process = Process.Start(info);
            if (redirectOutput && _process != null)
            {
                _process.OutputDataReceived += Process_OutputDataReceived;
                _process.ErrorDataReceived += Process_ErrorDataReceived;
                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();
            }
        }

        public bool WaitForExit(int timeout = -1)
        {
            var totalWait = 0;
            var slice = timeout == -1 ? timeout : Math.Max(timeout / 20, 1);
            do
            {
                if (_process.WaitForExit(slice))
                {
                    return true;
                }

                totalWait += slice;
            } while (timeout == -1 || totalWait < timeout);

            _process.KillTree(KillTimeOut);
            return false;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                _error.AppendLine(e.Data);
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                _output.AppendLine(e.Data);
            }
        }

        public void Dispose()
        {
            _process.OutputDataReceived -= Process_OutputDataReceived;
            _process.ErrorDataReceived -= Process_ErrorDataReceived;
            _process.Dispose();
        }
    }
}
