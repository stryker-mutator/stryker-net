using System.Diagnostics;
using System.IO;
using Xunit;

namespace Stryker.Core.IntegrationTest
{
    public class HappyFlow
    {
        [Fact]
        public void HappyFlowReturns0ExitCode()
        {
            int exitCode = -1;
            string errorOutput = "";

            string pathToExampleProject = Path.GetFullPath("../../../ExampleProject/ExampleProject.XUnit/");

            var processStartInfo = new ProcessStartInfo("dotnet", "stryker")
            {
                UseShellExecute = false,
                WorkingDirectory = pathToExampleProject,
                RedirectStandardOutput = false,
                RedirectStandardError = true
            };

            using (var process = Process.Start(processStartInfo))
            {

                errorOutput = process.StandardError.ReadToEnd();
                process.WaitForExit();

                exitCode = process.ExitCode;
            }

            Assert.True(exitCode == 0, "The stryker run failed on the integration test, error: " + errorOutput);
        }
    }
}