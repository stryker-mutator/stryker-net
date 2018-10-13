using Shouldly;
using Stryker.CLI;
using Stryker.Core.Options;
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

            var runner = new StrykerCLI(new StrykerRunner());

            runner.Run(new string[] { });

            Environment.ExitCode.ShouldBe(0);
        }
    }
}
