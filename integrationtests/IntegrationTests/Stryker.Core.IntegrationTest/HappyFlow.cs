using Shouldly;
using Stryker.Core.Options;
using System;
using Xunit;

namespace Stryker.Core.IntegrationTest
{
    public class HappyFlow
    {
        [Fact]
        public void HappyFlowReturns0ExitCode()
        {
            var runner = new StrykerRunner();
            string pathToExampleProject = "../../../ExampleProject/ExampleProject.XUnit/";
            var options = new StrykerOptions(pathToExampleProject, "Console", "", 30000, "", false, int.MaxValue, 80, 60, 0);
            runner.RunMutationTest(options);

            Environment.ExitCode.ShouldBe(0);
        }
    }
}
