using System.Collections.Generic;
using System.IO;
using Moq;
using Shouldly;
using Stryker.Core;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.CLI.UnitTest
{
    [Collection("StaticConfigBuilder")]
    public class ConfigFileTests
    {
        [Fact]
        public void WithNoArgumentsAndNoConfigFile_ShouldStartStrykerWithConfigOptions()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions()
            {
                Thresholds = new Thresholds()
                {
                    High = 80,
                    Low = 60,
                    Break = 0
                }
            };
            var currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory($"..{Path.DirectorySeparatorChar}");
            var runResults = new StrykerRunResult(options, 0.3);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults).Verifiable();
            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { });

            mock.VerifyAll();

            Directory.SetCurrentDirectory(currentDirectory);
        }

        [Theory]
        [InlineData("--config-file")]
        [InlineData("-f")]
        public void WithConfigFile_ShouldStartStrykerWithConfigFileOptions(string argName)
        {
            IStrykerInputs actualInputs = null;
            var options = new StrykerOptions() {
                Thresholds = new Thresholds() {
                    High = 80,
                    Low = 60,
                    Break = 0
                }
            };
            var runResults = new StrykerRunResult(options, 0.3);

            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<IStrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<IStrykerInputs, IEnumerable<LogMessage>>((c, m) => actualInputs = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "filled-stryker-config.json" });

            mock.VerifyAll();

            actualInputs.AdditionalTimeoutMsInput.SuppliedInput.ShouldBe(9999);
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
        }
    }
}
