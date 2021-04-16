using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Serilog.Events;
using Shouldly;
using Stryker.Core;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.CLI.UnitTest
{

    public class StrykerCLITests
    {
        private StrykerInputs inputs;
        private StrykerOptions options;
        private StrykerRunResult runResults;
        private Mock<IStrykerRunner> strykerRunnerMock = new Mock<IStrykerRunner>(MockBehavior.Strict);
        private StrykerCLI target;

        public StrykerCLITests()
        {
            options = new StrykerOptions() { Thresholds = new Thresholds { Break = 0 } };
            runResults = new StrykerRunResult(options, 0.3);
            strykerRunnerMock.Setup(x => x.RunMutationTest(It.IsAny<StrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<StrykerInputs, IEnumerable<LogMessage>>((c, m) => inputs = c)
                .Returns(runResults)
                .Verifiable();
            target = new StrykerCLI(strykerRunnerMock.Object);
        }

        [Fact]
        public void OnMutationScoreBelowThresholdBreak_ShouldReturnExitCode1()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions()
            {
                Thresholds = new Thresholds
                {
                    Break = 40
                }
            };
            var strykerRunResult = new StrykerRunResult(options, 0.3);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object);
            var result = target.Run(new string[] { });

            mock.Verify();
            target.ExitCode.ShouldBe(1);
            result.ShouldBe(1);
        }

        [Fact]
        public void OnMutationScoreEqualToNullAndThresholdBreakEqualTo0_ShouldReturnExitCode0()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions()
            {
                Thresholds = new Thresholds
                {
                    Break = 0
                }
            };
            var strykerRunResult = new StrykerRunResult(options, double.NaN);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object);
            var result = target.Run(new string[] { });

            mock.Verify();
            target.ExitCode.ShouldBe(0);
            result.ShouldBe(0);
        }

        [Fact]
        public void OnMutationScoreEqualToNullAndThresholdBreakAbove0_ShouldReturnExitCode0()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions()
            {
                Thresholds = new Thresholds
                {
                    Break = 40
                }
            };
            var strykerRunResult = new StrykerRunResult(options, double.NaN);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object);
            var result = target.Run(new string[] { });

            mock.Verify();
            target.ExitCode.ShouldBe(0);
            result.ShouldBe(0);
        }

        [Fact]
        public void OnMutationScoreAboveThresholdBreak_ShouldReturnExitCode0()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions()
            {
                Thresholds = new Thresholds
                {
                    Break = 0
                }
            };
            var strykerRunResult = new StrykerRunResult(options, 0.1);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object);
            var result = target.Run(new string[] { });

            mock.Verify();
            target.ExitCode.ShouldBe(0);
            result.ShouldBe(0);
        }

        [Theory]
        [InlineData("--help")]
        [InlineData("-h")]
        [InlineData("-?")]
        public void ShouldNotStartStryker_WithHelpArgument(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName });

            mock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrow_OnException()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>())).Throws(new Exception("Initial testrun failed")).Verifiable();

            var target = new StrykerCLI(mock.Object);
            Assert.Throws<Exception>(() => target.Run(new string[] { }));
        }

        [Theory]
        [InlineData("--reporter")]
        [InlineData("-r")]
        public void ShouldPassReporterArgumentsToStryker_WithReporterArgument(string argName)
        {
            target.Run(new string[] { argName, Reporter.Html.ToString(), argName, Reporter.Dots.ToString() });

            strykerRunnerMock.VerifyAll();

            inputs.ReportersInput.SuppliedInput.ShouldContain(Reporter.Html.ToString());
            inputs.ReportersInput.SuppliedInput.ShouldContain(Reporter.Dots.ToString());
        }

        [Theory]
        [InlineData("--project")]
        [InlineData("-p")]
        public void ShouldPassProjectArgumentsToStryker_WithProjectArgument(string argName)
        {
            target.Run(new string[] { argName, "SomeProjectName.csproj" });

            strykerRunnerMock.VerifyAll();

            inputs.ProjectUnderTestNameInput.SuppliedInput.ShouldBe("SomeProjectName.csproj");
        }

        [Theory]
        [InlineData("--solution")]
        [InlineData("-s")]
        public void ShouldPassSolutionArgumentPlusBasePathToStryker_WithSolutionArgument(string argName)
        {
            target.Run(new string[] { argName, "SomeSolutionPath.sln" });

            strykerRunnerMock.VerifyAll();

            inputs.SolutionPathInput.SuppliedInput.ShouldBe("SomeSolutionPath.sln");
        }

        [Theory]
        [InlineData("--verbosity")]
        [InlineData("-V")]
        public void ShouldPassLogConsoleArgumentsToStryker_WithLogConsoleArgument(string argName)
        {
            target.Run(new[] { argName, "Debug" });

            strykerRunnerMock.VerifyAll();

            inputs.VerbosityInput.SuppliedInput.ShouldBe(LogEventLevel.Debug.ToString());
        }

        [Theory]
        [InlineData("--log-to-file")]
        [InlineData("-L")]
        public void ShouldPassLogFileArgumentsToStryker_WithLogLevelFileArgument(string argName)
        {
            target.Run(new string[] { argName });

            strykerRunnerMock.VerifyAll();

            inputs.LogToFileInput.SuppliedInput.Value.ShouldBeTrue();
        }

        [Theory]
        [InlineData("--dev-mode")]
        public void WithDevModeArgument_ShouldPassDevModeArgumentsToStryker(string argName)
        {
            target.Run(new string[] { argName });

            strykerRunnerMock.VerifyAll();

            inputs.DevModeInput.SuppliedInput.Value.ShouldBeTrue();
        }

        [Theory]
        [InlineData("--concurrency")]
        [InlineData("-c")]
        public void WithMaxConcurrentTestrunnerArgument_ShouldPassValidatedConcurrentTestrunnersToStryker(string argName)
        {
            target.Run(new string[] { argName, "4" });

            strykerRunnerMock.VerifyAll();

            inputs.ConcurrencyInput.SuppliedInput.Value.ShouldBe(4);
        }

        [Theory]
        [InlineData("--break")]
        [InlineData("-b")]
        public void WithCustomThresholdBreakParameter_ShouldPassThresholdBreakToStryker(string argName)
        {
            target.Run(new string[] { argName, "20" });

            strykerRunnerMock.VerifyAll();

            inputs.ThresholdBreakInput.SuppliedInput.ShouldBe(20);
        }

        [Theory]
        [InlineData("--mutate")]
        [InlineData("-m")]
        public void ShouldPassFilePatternSetToStryker_WithMutateArgs(string argName)
        {
            var firstFileToExclude = "**/*Service.cs";
            var secondFileToExclude = "!**/MySpecialService.cs";
            var thirdFileToExclude = "**/MyOtherService.cs{1..10}{32..45}";

            target.Run(new[] { argName, firstFileToExclude, argName, secondFileToExclude, argName, thirdFileToExclude });

            strykerRunnerMock.VerifyAll();

            var filePatterns = inputs.MutateInput.SuppliedInput.ToArray();
            filePatterns.Length.ShouldBe(3);
            filePatterns.ShouldContain(firstFileToExclude);
            filePatterns.ShouldContain(secondFileToExclude);
            filePatterns.ShouldContain(thirdFileToExclude);
        }

        [Theory]
        [InlineData("--since")]
        [InlineData("-since")]
        public void ShouldEnableDiffFeatureWhenPassed(string argName)
        {
            target.Run(new string[] { argName });

            strykerRunnerMock.VerifyAll();

            inputs.SinceInput.SuppliedInput.Value.ShouldBeTrue();
        }

        [Theory]
        [InlineData("--since")]
        [InlineData("-since")]
        public void ShouldSetGitDiffTargetWhenPassed(string argName)
        {
            target.Run(new string[] { $"{argName}:development" });

            strykerRunnerMock.VerifyAll();

            inputs.SinceInput.SuppliedInput.Value.ShouldBeTrue();
            inputs.SinceTargetInput.SuppliedInput.ShouldBe("development");
        }

        [Theory]
        [InlineData("--mutation-level")]
        [InlineData("-l")]
        public void ShouldSetMutationLevelWhenPassed(string argName)
        {
            target.Run(new string[] { argName, "Advanced" });

            inputs.MutationLevelInput.SuppliedInput.ShouldBe(MutationLevel.Advanced.ToString());
        }

        [Theory]
        [InlineData("--version", "master")]
        [InlineData("-v", "master")]
        public void ShouldEnableDiffCompareToDashboardFeatureWhenPassed(params string[] argName)
        {
            target.Run(argName);

            strykerRunnerMock.VerifyAll();

            inputs.ProjectVersionInput.SuppliedInput.ShouldBe("master");
        }

        [Theory]
        [InlineData("--dashboard-api-key", "1234567890")]
        public void ShouldSupplyDashboardApiKeyWhenPassed(params string[] argName)
        {
            target.Run(argName);

            strykerRunnerMock.VerifyAll();

            inputs.DashboardApiKeyInput.SuppliedInput.ShouldBe("1234567890");
        }

        [Theory]
        [InlineData("--with-baseline")]
        public void ShouldSupplyWithBaselineWhenPassed(params string[] argName)
        {
            target.Run(argName);

            strykerRunnerMock.VerifyAll();

            inputs.WithBaselineInput.SuppliedInput.Value.ShouldBeTrue();
        }

        [Theory]
        [InlineData("--azure-fileshare-sas", "sas")]
        public void ShouldSupplyAzureFileshareSasWhenPassed(params string[] argName)
        {
            target.Run(argName);

            strykerRunnerMock.VerifyAll();

            inputs.AzureFileStorageSasInput.SuppliedInput.ShouldBe("sas");
        }
    }
}
