using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Stryker.Core.Testing
{
    [ExcludeFromCodeCoverage]
    public class ProcessExecutor : IProcessExecutor
    {
        private bool _redirectOutput { get; set; }

        public ProcessExecutor(bool redirectOutput = true)
        {
            _redirectOutput = redirectOutput;
        }

        public ProcessResult Start(
            string path, 
            string application, 
            string arguments, 
            IEnumerable<KeyValuePair<string, string>> environmentVariables = null)
        {
            var info = new ProcessStartInfo(application, arguments)
            {
                UseShellExecute = false,
                WorkingDirectory = path,
                RedirectStandardOutput = _redirectOutput,
                RedirectStandardError = _redirectOutput
            };

            foreach(var environmentVariable in environmentVariables ?? Enumerable.Empty<KeyValuePair<string, string>>())
            {
                info.EnvironmentVariables[environmentVariable.Key] = environmentVariable.Value;
            }

            var process = Process.Start(info);
            string output = "";
            if(_redirectOutput)
            {
                output = process.StandardOutput.ReadToEnd();
            }
            process.WaitForExit();

            return new ProcessResult()
            {
                ExitCode = process.ExitCode,
                Output = output
            };
        }
    }
}
