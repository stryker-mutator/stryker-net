using Moq;
using Serilog.Events;
using Shouldly;
using Stryker.Core;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Stryker.CLI.UnitTest
{

    public class StrykerCLITests
    {
        private readonly ITestOutputHelper _output;
        private string _fileSystemRoot { get; }

        private string _currentDirectory { get; }

        public StrykerCLITests(ITestOutputHelper output)
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _fileSystemRoot = Path.GetPathRoot(_currentDirectory);
            _output = output;
        }

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
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Throws(new Exception("Initial testrun failed")).Verifiable();

            var target = new StrykerCLI(mock.Object);
            Assert.Throws<Exception>(() => target.Run(new string[] { }));
        }

        [Theory]
        [InlineData("--reporters")]
        [InlineData("-r")]
        public void StrykerCLI_WithReporterArgument_ShouldPassReporterArgumentsToStryker(string argName)
        {
            StrykerOptions options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3M);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, $"['{ Reporter.ConsoleReport }', '{ Reporter.ConsoleProgressDots }']" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.Reporters.Contains(Reporter.ConsoleReport) && o.Reporters.Contains(Reporter.ConsoleProgressDots))));
        }


        [Theory]
        [InlineData("--excluded-mutations")]
        [InlineData("-em")]
        public void StrykerCLI_WithExcludedMutationsArgument_ShouldPassExcludedMutationsArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3M);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "['string', 'logical']" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.ExcludedMutations.Contains(Mutator.String) &&
                o.ExcludedMutations.Contains(Mutator.Logical)
            )));
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
            StrykerOptions options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3M);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            argValues.Count().ShouldBeGreaterThan(0);

            foreach (string argValue in argValues)
            {
                target.Run(new string[] { "-em", $"['{argValue}']" });

                mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.ExcludedMutations.Single() == expectedType)));
            }
        }

        [Theory]
        [InlineData("--project-file")]
        [InlineData("-p")]
        public void StrykerCLI_WithProjectArgument_ShouldPassProjectArgumentsToStryker(string argName)
        {
            StrykerOptions options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3M);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "SomeProjectName.csproj" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.ProjectUnderTestNameFilter == "SomeProjectName.csproj")));
        }

        [Theory]
        [InlineData("--solution-path")]
        [InlineData("-s")]
        public void StrykerCLI_WithSolutionArgument_ShouldPassSolutionArgumentPlusBasePathToStryker(string argName)
        {
            StrykerOptions options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3M);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "SomeSolutionPath.sln" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.SolutionPath.Contains("SomeSolutionPath.sln"))));
        }

        [Theory]
        [InlineData("--log-level")]
        [InlineData("-l")]
        public void StrykerCLI_WithLogConsoleArgument_ShouldPassLogConsoleArgumentsToStryker(string argName)
        {
            StrykerOptions actualOptions = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.3M);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()))
                .Callback<StrykerOptions>((c) => actualOptions = c)
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
            StrykerOptions options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3M);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.LogOptions.LogToFile)));
        }

        [Theory]
        [InlineData("--dev-mode")]
        public void StrykerCLI_WithDevModeArgument_ShouldPassDevModeArgumentsToStryker(string argName)
        {
            StrykerOptions actualOptions = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.3M);

            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()))
                .Callback<StrykerOptions>((c) => actualOptions = c)
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
            StrykerOptions options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3M);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "1000" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.AdditionalTimeoutMS == 1000)));
        }

        [Theory]
        [InlineData("--max-concurrent-test-runners")]
        [InlineData("-c")]
        public void StrykerCLI_WithMaxConcurrentTestrunnerArgument_ShouldPassValidatedConcurrentTestrunnersToStryker(string argName)
        {
            StrykerOptions options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3M);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "4" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.ConcurrentTestrunners <= 4)));
        }

        [Theory]
        [InlineData("--threshold-break")]
        [InlineData("-tb")]
        public void StrykerCLI_WithCustomThresholdBreakParameter_ShouldPassThresholdBreakToStryker(string argName)
        {
            StrykerOptions options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3M);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "20" });
            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.Thresholds.Break == 20)));
        }

        [Theory]
        [InlineData("--threshold-low")]
        [InlineData("-tl")]
        public void StrykerCLI_WithCustomThresholdLowParameter_ShouldPassThresholdLowToStryker(string argName)
        {
            StrykerOptions options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3M);
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "65" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.Thresholds.Low == 65)));
        }

        [Theory]
        [InlineData("--threshold-high")]
        [InlineData("-th")]
        public void StrykerCLI_WithCustomThresholdHighParameter_ShouldPassThresholdHighToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions options = new StrykerOptions();
            var runResult = new StrykerRunResult(options, 0.3M);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResult);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "90" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.Thresholds.High == 90)));
        }

        [Fact]
        public void StrykerCLI_OnMutationScoreBelowThresholdBreak_ShouldReturnExitCode1()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions options = new StrykerOptions(thresholdBreak: 40);
            StrykerRunResult strykerRunResult = new StrykerRunResult(options, 0.3M);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object);
            int result = target.Run(new string[] { });

            mock.Verify();
            target.ExitCode.ShouldBe(1);
            result.ShouldBe(1);
        }

        [Fact]
        public void StrykerCLI_OnMutationScoreEqualToNullAndThresholdBreakEqualTo0_ShouldReturnExitCode0()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions options = new StrykerOptions(thresholdBreak: 0);
            StrykerRunResult strykerRunResult = new StrykerRunResult(options, null);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object);
            int result = target.Run(new string[] { });

            mock.Verify();
            target.ExitCode.ShouldBe(0);
            result.ShouldBe(0);
        }

        [Fact]
        public void StrykerCLI_OnMutationScoreEqualToNullAndThresholdBreakAbove0_ShouldReturnExitCode0()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions options = new StrykerOptions(thresholdBreak: 40);
            StrykerRunResult strykerRunResult = new StrykerRunResult(options, null);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object);
            int result = target.Run(new string[] { });

            mock.Verify();
            target.ExitCode.ShouldBe(0);
            result.ShouldBe(0);
        }

        [Fact]
        public void StrykerCLI_OnMutationScoreAboveThresholdBreak_ShouldReturnExitCode0()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions options = new StrykerOptions(thresholdBreak: 0);
            StrykerRunResult strykerRunResult = new StrykerRunResult(options, 0.1M);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object);
            int result = target.Run(new string[] { });

            mock.Verify();
            target.ExitCode.ShouldBe(0);
            result.ShouldBe(0);
        }

        [Fact]
        public void StrykerCLI_WithNoFilesToExcludeSet_ShouldPassDefaultValueToStryker()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions options = new StrykerOptions();
            StrykerRunResult strykerRunResult = new StrykerRunResult(options, 0.1M);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(() => strykerRunResult);

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                !o.FilesToExclude.Any())));
        }

        [Theory]
        [InlineData("--files-to-exclude")]
        [InlineData("-fte")]
        public void StrykerCLI_WithFilesToExcludeSet_ShouldPassFilesToExcludeToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions actualOptions = null;
            StrykerRunResult runResults = new StrykerRunResult(new StrykerOptions(), 0.1M);

            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()))
                .Callback<StrykerOptions>((c) => actualOptions = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new[] { argName, @"['./StartUp.cs','./ExampleDirectory/Recursive.cs', '.\\ExampleDirectory/Recursive2.cs']" });

            var firstFileToExclude = FilePathUtils.ConvertPathSeparators("./StartUp.cs");
            var secondFileToExclude = FilePathUtils.ConvertPathSeparators("./ExampleDirectory/Recursive.cs");
            var thirdFileToExclude = FilePathUtils.ConvertPathSeparators(@".\ExampleDirectory/Recursive2.cs");

            var filesToExclude = actualOptions.FilesToExclude.ToArray();
            filesToExclude.Length.ShouldBe(3);
            filesToExclude.ShouldContain(firstFileToExclude);
            filesToExclude.ShouldContain(secondFileToExclude);
            filesToExclude.ShouldContain(thirdFileToExclude);
        }
    }
}
