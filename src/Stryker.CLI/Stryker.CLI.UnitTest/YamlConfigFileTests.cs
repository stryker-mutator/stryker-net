using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.CLI.UnitTest
{
    public class YamlConfigFileTests
    {


        [Fact]
        public void YamlConfigFile_IsLoadedCorrectly()
        {
            IStrykerInputs actualInputs = null;
            var options = new StrykerOptions()
            {
                Thresholds = new Thresholds()
                {
                    High = 80,
                    Low = 60,
                    Break = 0
                }
            };
            var runResults = new StrykerRunResult(options, 0.3);

            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<IStrykerInputs>(), It.IsAny<ILoggerFactory>(), It.IsAny<IProjectOrchestrator>()))
                .Callback<IStrykerInputs, ILoggerFactory, IProjectOrchestrator>((c, l, p) => actualInputs = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCli(mock.Object);

            target.Run(new string[] { "-f", "filled-stryker-config.yaml" });

            mock.VerifyAll();

            actualInputs.AdditionalTimeoutInput.SuppliedInput.ShouldBe(9999);
            actualInputs.VerbosityInput.SuppliedInput.ShouldBe("trace");
            actualInputs.ProjectUnderTestNameInput.SuppliedInput.ShouldBe("ExampleProject.csproj");
            actualInputs.ReportersInput.SuppliedInput.ShouldHaveSingleItem();
            actualInputs.ReportersInput.SuppliedInput.ShouldContain(Reporter.Json.ToString());
            actualInputs.ConcurrencyInput.SuppliedInput.ShouldBe(1);
            actualInputs.ThresholdBreakInput.SuppliedInput.ShouldBe(20);
            actualInputs.ThresholdLowInput.SuppliedInput.ShouldBe(30);
            actualInputs.ThresholdHighInput.SuppliedInput.ShouldBe(40);
            actualInputs.MutateInput.SuppliedInput.ShouldHaveSingleItem();
            actualInputs.MutateInput.SuppliedInput.ShouldContain("!**/Test.cs{1..100}{200..300}");
            actualInputs.CoverageAnalysisInput.SuppliedInput.ShouldBe("perTest");
            actualInputs.DisableBailInput.SuppliedInput.ShouldBe(true);
            actualInputs.IgnoreMutationsInput.SuppliedInput.ShouldContain("linq.FirstOrDefault");
            actualInputs.IgnoredMethodsInput.SuppliedInput.ShouldContain("Log*");
            actualInputs.TestCaseFilterInput.SuppliedInput.ShouldBe("(FullyQualifiedName~UnitTest1&TestCategory=CategoryA)|Priority=1");
            actualInputs.DashboardUrlInput.SuppliedInput.ShouldBe("https://alternative-stryker-dashboard.io");
        }
    }
}
