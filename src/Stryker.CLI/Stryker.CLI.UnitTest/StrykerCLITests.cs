using Moq;
using Serilog.Events;
using Shouldly;
using Stryker.Core;
using Stryker.Core.Baseline;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stryker.CLI.UnitTest
{

    public class StrykerCLITests
    {
        [Theory]
        [InlineData("--help")]
        [InlineData("-h")]
        [InlineData("-?")]
        public void StrykerCLI_WithHelpArgument_ShouldNotStartStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName });

            mock.VerifyNoOtherCalls();
        }

        [Fact]
        public void StrykerCLI_OnException_ShouldThrow()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Throws(new Exception("Initial testrun failed")).Verifiable();

            var target = new StrykerCLI(mock.Object);
            Assert.Throws<Exception>(() => target.Run(new string[] { }));
        }

        [Theory]
        [InlineData("--reporters")]
        [InlineData("-r")]
        public void StrykerCLI_WithReporterArgument_ShouldPassReporterArgumentsToStryker(string argName)
        {
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, $"['{ Reporter.ConsoleReport }', '{ Reporter.ConsoleProgressDots }']" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.Reporters.Contains(Reporter.ConsoleReport) && o.Reporters.Contains(Reporter.ConsoleProgressDots)), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--excluded-mutations")]
        [InlineData("-em")]
        public void StrykerCLI_WithExcludedMutationsArgument_ShouldPassExcludedMutationsArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "['string', 'logical']" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.ExcludedMutations.Contains(Mutator.String) &&
                o.ExcludedMutations.Contains(Mutator.Logical)
            ), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData(Mutator.Assignment, "assignment", "assignment statements")]
        [InlineData(Mutator.Arithmetic, "arithmetic", "arithmetic operators")]
        [InlineData(Mutator.Boolean, "boolean", "boolean literals")]
        [InlineData(Mutator.Equality, "equality", "equality operators")]
        [InlineData(Mutator.Linq, "linq", "linq methods")]
        [InlineData(Mutator.Logical, "logical", "logical operators")]
        [InlineData(Mutator.String, "string", "string literals")]
        [InlineData(Mutator.Unary, "unary", "unary operators")]
        [InlineData(Mutator.Update, "update", "update operators")]
        public void StrykerCLI_ExcludedMutationsNamesShouldMapToMutatorTypes(Mutator expectedType, params string[] argValues)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            argValues.Count().ShouldBeGreaterThan(0);

            foreach (var argValue in argValues)
            {
                target.Run(new string[] { "-em", $"['{argValue}']" });

                mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.ExcludedMutations.Single() == expectedType), It.IsAny<IEnumerable<LogMessage>>()));
            }
        }

        [Theory]
        [InlineData("--project-file")]
        [InlineData("-p")]
        public void StrykerCLI_WithProjectArgument_ShouldPassProjectArgumentsToStryker(string argName)
        {
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "SomeProjectName.csproj" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.ProjectUnderTestNameFilter == "SomeProjectName.csproj"), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--test-projects")]
        [InlineData("-tp")]
        public void StrykerCLI_WithTestProjectsArgument_ShouldPassTestProjectArgumentsToStryker(string argName)
        {
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "['TestProjectFolder/SomeTestProjectName.csproj']" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.TestProjects.Count() == 1), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--solution-path")]
        [InlineData("-s")]
        public void StrykerCLI_WithSolutionArgument_ShouldPassSolutionArgumentPlusBasePathToStryker(string argName)
        {
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "SomeSolutionPath.sln" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.SolutionPath.Contains("SomeSolutionPath.sln")), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--log-level")]
        [InlineData("-l")]
        public void StrykerCLI_WithLogConsoleArgument_ShouldPassLogConsoleArgumentsToStryker(string argName)
        {
            StrykerOptions actualOptions = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.3);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<StrykerOptions, IEnumerable<LogMessage>>((c, m) => actualOptions = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new[] { argName, "debug" });

            mock.VerifyAll();
            actualOptions.LogOptions.LogLevel.ShouldBe(LogEventLevel.Debug);
            actualOptions.LogOptions.LogToFile.ShouldBeFalse();
        }

        [Theory]
        [InlineData("--log-file")]
        public void StrykerCLI_WithLogLevelFileArgument_ShouldPassLogFileArgumentsToStryker(string argName)
        {
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.LogOptions.LogToFile), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--dev-mode")]
        public void StrykerCLI_WithDevModeArgument_ShouldPassDevModeArgumentsToStryker(string argName)
        {
            StrykerOptions actualOptions = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.3);

            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<StrykerOptions, IEnumerable<LogMessage>>((c, m) => actualOptions = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName });

            mock.VerifyAll();

            actualOptions.DevMode.ShouldBeTrue();
        }

        [Theory]
        [InlineData("--timeout-ms")]
        [InlineData("-t")]
        public void StrykerCLI_WithTimeoutArgument_ShouldPassTimeoutToStryker(string argName)
        {
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "1000" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.AdditionalTimeoutMS == 1000), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--max-concurrent-test-runners")]
        [InlineData("-c")]
        public void StrykerCLI_WithMaxConcurrentTestrunnerArgument_ShouldPassValidatedConcurrentTestrunnersToStryker(string argName)
        {
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "4" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.ConcurrentTestrunners <= 4), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--threshold-break")]
        [InlineData("-tb")]
        public void StrykerCLI_WithCustomThresholdBreakParameter_ShouldPassThresholdBreakToStryker(string argName)
        {
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "20" });
            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.Thresholds.Break == 20), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--threshold-low")]
        [InlineData("-tl")]
        public void StrykerCLI_WithCustomThresholdLowParameter_ShouldPassThresholdLowToStryker(string argName)
        {
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "65" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.Thresholds.Low == 65), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--threshold-high")]
        [InlineData("-th")]
        public void StrykerCLI_WithCustomThresholdHighParameter_ShouldPassThresholdHighToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions();
            var runResult = new StrykerRunResult(options, 0.3);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResult);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "90" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.Thresholds.High == 90), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Fact]
        public void StrykerCLI_OnMutationScoreBelowThresholdBreak_ShouldReturnExitCode1()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions(thresholdBreak: 40);
            var strykerRunResult = new StrykerRunResult(options, 0.3);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object);
            var result = target.Run(new string[] { });

            mock.Verify();
            target.ExitCode.ShouldBe(1);
            result.ShouldBe(1);
        }

        [Fact]
        public void StrykerCLI_OnMutationScoreEqualToNullAndThresholdBreakEqualTo0_ShouldReturnExitCode0()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions(thresholdBreak: 0);
            var strykerRunResult = new StrykerRunResult(options, double.NaN);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object);
            var result = target.Run(new string[] { });

            mock.Verify();
            target.ExitCode.ShouldBe(0);
            result.ShouldBe(0);
        }

        [Fact]
        public void StrykerCLI_OnMutationScoreEqualToNullAndThresholdBreakAbove0_ShouldReturnExitCode0()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions(thresholdBreak: 40);
            var strykerRunResult = new StrykerRunResult(options, double.NaN);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object);
            var result = target.Run(new string[] { });

            mock.Verify();
            target.ExitCode.ShouldBe(0);
            result.ShouldBe(0);
        }

        [Fact]
        public void StrykerCLI_OnMutationScoreAboveThresholdBreak_ShouldReturnExitCode0()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions(thresholdBreak: 0);
            var strykerRunResult = new StrykerRunResult(options, 0.1);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object);
            var result = target.Run(new string[] { });

            mock.Verify();
            target.ExitCode.ShouldBe(0);
            result.ShouldBe(0);
        }

        [Fact]
        public void StrykerCLI_WithNoFilesToExcludeSet_ShouldPassDefaultValueToStryker()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions();
            var strykerRunResult = new StrykerRunResult(options, 0.1);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(() => strykerRunResult);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.FilePatterns.Count() == 1), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--files-to-exclude")]
        [InlineData("-fte")]
        public void StrykerCLI_WithFilesToExcludeSet_ShouldPassFilesToExcludeToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions actualOptions = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.1);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<StrykerOptions, IEnumerable<LogMessage>>((c, m) => actualOptions = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new[] { argName, @"['./StartUp.cs','./ExampleDirectory/Recursive.cs', './ExampleDirectory/Recursive2.cs']" });

            var firstFileToExclude = FilePattern.Parse("!StartUp.cs");
            var secondFileToExclude = FilePattern.Parse("!ExampleDirectory/Recursive.cs");
            var thirdFileToExclude = FilePattern.Parse("!ExampleDirectory/Recursive2.cs");

            var filePatterns = actualOptions.FilePatterns.ToArray();
            filePatterns.Count(x => x.IsExclude).ShouldBe(3);
            filePatterns.ShouldContain(firstFileToExclude);
            filePatterns.ShouldContain(secondFileToExclude);
            filePatterns.ShouldContain(thirdFileToExclude);
        }

        [Theory]
        [InlineData("--mutate")]
        [InlineData("-m")]
        public void StrykerCLI_WithFilePatternSet_ShouldPassFilePatternSetToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions actualOptions = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.1);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<StrykerOptions, IEnumerable<LogMessage>>((c, m) => actualOptions = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new[] { argName, @"['**/*Service.cs','!**/MySpecialService.cs', '**/MyOtherService.cs{1..10}{32..45}']" });

            var firstFileToExclude = FilePattern.Parse("**/*Service.cs");
            var secondFileToExclude = FilePattern.Parse("!**/MySpecialService.cs");
            var thirdFileToExclude = FilePattern.Parse("**/MyOtherService.cs{1..10}{32..45}");

            var filePatterns = actualOptions.FilePatterns.ToArray();
            filePatterns.Length.ShouldBe(3);
            filePatterns.ShouldContain(firstFileToExclude);
            filePatterns.ShouldContain(secondFileToExclude);
            filePatterns.ShouldContain(thirdFileToExclude);
        }

        [Theory]
        [InlineData("--diff")]
        [InlineData("-diff")]
        public void ShouldEnableDiffFeatureWhenPassed(string argName)
        {
            StrykerOptions actualOptions = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.3);

            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<StrykerOptions, IEnumerable<LogMessage>>((c, m) => actualOptions = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName });

            mock.VerifyAll();

            actualOptions.DiffEnabled.ShouldBeTrue();
        }

        [Theory]
        [InlineData("--mutation-level")]
        [InlineData("-level")]
        public void ShouldSetMutationLevelWhenPassed(string argName)
        {
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "advanced" });
            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.MutationLevel == MutationLevel.Advanced), It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--dashboard-compare", "--dashboard-version project")]
        [InlineData("-compare", "-version project")]
        public void ShouldEnableDiffCompareToDashboardFeatureWhenPassed(params string[] argName)
        {
            StrykerOptions options = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.3);

            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<StrykerOptions, IEnumerable<LogMessage>>((c, m) => options = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(argName);

            mock.VerifyAll();

            options.CompareToDashboard.ShouldBeTrue();
        }

        [Theory]
        [InlineData("--dashboard-compare", "--dashboard-version project")]
        [InlineData("-compare", "-version project")]
        public void ShouldEnableDiffFeatureWhenDashboardComparePassed(params string[] argNames)
        {
            StrykerOptions options = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.3);

            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<StrykerOptions, IEnumerable<LogMessage>>((c, m) => options = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(argNames);

            mock.VerifyAll();

            options.DiffEnabled.ShouldBeTrue();
        }

        [Theory]
        [InlineData("--dashboard-url https://www.example.com/")]
        [InlineData("-url https://www.example.com/")]
        public void ShouldOverwriteDefaultDashboardUrlWhenPassed(string argName)
        {
            StrykerOptions options = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.3);

            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<StrykerOptions, IEnumerable<LogMessage>>((c, m) => options = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "--reporters", "['dashboard']", "--dashboard-project", "test", "--dashboard-api-key", "test" });

            mock.VerifyAll();

            options.DashboardUrl.ShouldBe("https://www.example.com/");
        }

        [Fact]
        public void ShouldKeepDefaultDashboardUrlWhenArgumentNotProvided()
        {
            StrykerOptions options = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.3);

            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<StrykerOptions, IEnumerable<LogMessage>>((c, m) => options = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { "--reporters", "['dashboard']", "--dashboard-project", "test", "--dashboard-api-key", "test" });

            mock.VerifyAll();

            options.DashboardUrl.ShouldBe("https://dashboard.stryker-mutator.io");
        }

        [Theory]
        [InlineData("--git-diff-target")]
        [InlineData("-gdt")]
        public void ShouldSetGitDiffTargetWhenPassed(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "development" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.GitDiffSource == "development"),
                It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--baseline-storage-location disk")]
        [InlineData("-bsl disk")]
        public void ShouldSetDiskBaselineProviderWhenSpecified(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.BaselineProvider == BaselineProvider.Disk),
                It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--baseline-storage-location dashboard")]
        [InlineData("-bsl dashboard")]
        public void ShouldSetDashboardBaselineProviderWhenSpecified(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.BaselineProvider == BaselineProvider.Dashboard),
                It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Fact]
        public void ShouldSetDiskBaselineProviderWhenNotSpecifiedAndNoDashboardReporterSpecified()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.BaselineProvider == BaselineProvider.Disk),
                It.IsAny<IEnumerable<LogMessage>>()));
        }

        [Theory]
        [InlineData("--diff-ignore-files ['**/*.ts']")]
        [InlineData("-diffignorefiles ['**/*.ts']")]
        public void ShouldCreateDiffIgnoreGlobFiltersIfSpecified(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.DiffIgnoreFiles.Count() == 1),
                It.IsAny<IEnumerable<LogMessage>>()));
        }
    }
}
