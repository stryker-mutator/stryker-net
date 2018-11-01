using Shouldly;
using Stryker.CLI;
using System;
using System.IO;
using Xunit;

namespace Stryker.Core.IntegrationTest
{
    public class HappyFlow
    {
        [Fact]
        public void HappyFlowReturns0ExitCode()
        {
            string pathToExampleProject = Path.GetFullPath("../../../ExampleProject/ExampleProject.XUnit/");
            Environment.CurrentDirectory = pathToExampleProject;

            int exitCode = Program.Main(new string[] { "--log-console", "info" });

            exitCode.ShouldBe(0);
        }
    }
}
