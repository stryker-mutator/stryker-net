using Serilog.Events;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Xunit;
using Stryker.Core.Reporters;
using System;
using Stryker.Core.TestRunners;

namespace Stryker.Core.UnitTest.Options
{
    public class StrykerOptionsTests
    {
        [Fact]
        public void ShouldContainCorrectDefaults()
        {
            var options = new StrykerOptions();

            options.DashboardUrl.ShouldBe("https://dashboard.stryker-mutator.io");
        }

        [Theory]
        [InlineData("error", LogEventLevel.Error)]
        [InlineData("", LogEventLevel.Information)]
        [InlineData(null, LogEventLevel.Information)]
        [InlineData("warning", LogEventLevel.Warning)]
        [InlineData("info", LogEventLevel.Information)]
        [InlineData("debug", LogEventLevel.Debug)]
        [InlineData("trace", LogEventLevel.Verbose)]
        public void Constructor_WithCorrectLoglevelArgument_ShouldAssignCorrectLogLevel(string argValue, LogEventLevel expectedLogLevel)
        {
            var options = new StrykerOptions(logLevel: argValue);

            options.LogOptions.ShouldNotBeNull();
            options.LogOptions.LogLevel.ShouldBe(expectedLogLevel);
        }

        [Fact]
        public void Constructor_WithIncorrectLoglevelArgument_ShouldThrowStrykerInputException()
        {
            var logLevel = "incorrect";

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerOptions(logLevel: logLevel);
            });

            ex.Message.ShouldBe("The value for one of your settings is not correct. Try correcting or removing them.");
        }

        [Fact]
        public void Constructor_WithIncorrectSettings_ShoudThrowWithDetails()
        {
            var logLevel = "incorrect";

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerOptions(logLevel: logLevel);
            });

            ex.Details.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void CustomTestProjectFilter_WithRelativePath_ShouldIncludeBasePath()
        {
            var userSuppliedFilter = "..\\ExampleActualTestProject\\TestProject.csproj";
            var basePath = FilePathUtils.NormalizePathSeparators(Path.Combine("C:", "ExampleProject", "TestProject"));
            var fullPath = FilePathUtils.NormalizePathSeparators(Path.Combine("C:", "ExampleProject", "ExampleActualTestProject", "TestProject.csproj"));

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerOptions(
                    basePath: basePath,
                    testProjectNameFilter: userSuppliedFilter);
            });

            ex.Details.ShouldContain($"The test project filter {userSuppliedFilter} is invalid.");
        }

        [Theory]
        [InlineData("./MyFolder/MyFile.cs", "MyFolder/MyFile.cs")]
        [InlineData("./MyFolder", "MyFolder/*.*")]
        [InlineData("C:/MyFolder/MyFile.cs", "C:/MyFolder/MyFile.cs")]
        public void FilesToExclude_should_be_converted_to_file_patterns(string fileToExclude, string expectedFilePattern)
        {
            // Act
            var result = new StrykerOptions(filesToExclude: new []{fileToExclude});

            // Assert
            var pattern = result.FilePatterns.Last();
            Path.GetFullPath(pattern.Glob.ToString()).ShouldBe(Path.GetFullPath(expectedFilePattern));
            pattern.TextSpans.ShouldContain(TextSpan.FromBounds(0, int.MaxValue));
            pattern.IsExclude.ShouldBeTrue();
        }

        [Fact]
        public void ShouldValidateApiKey()
        {
            var options = new StrykerOptions();

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerOptions(reporters: new string[] { "Dashboard" });
            });
            ex.Message.ShouldContain($"An API key is required when the {Reporter.Dashboard} reporter is turned on! You can get an API key at {options.DashboardUrl}");
            ex.Message.ShouldContain($"A project name is required when the {Reporter.Dashboard} reporter is turned on!");
        }

        [Fact]
        public void ShouldValidateGitSource()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerOptions(gitSource: "");
            });
            ex.Message.ShouldBe("GitSource may not be empty, please provide a valid git branch name");
        }

        [Fact]
        public void ShouldValidateOptimisationMode()
        {
            var options = new StrykerOptions(coverageAnalysis: "perTestInIsolation");
            options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest).ShouldBeTrue();
            options.Optimizations.HasFlag(OptimizationFlags.CaptureCoveragePerTest).ShouldBeTrue();

            options = new StrykerOptions();
            options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest).ShouldBeTrue();

            options = new StrykerOptions(coverageAnalysis: "all");
            options.Optimizations.HasFlag(OptimizationFlags.SkipUncoveredMutants).ShouldBeTrue();

            options = new StrykerOptions(coverageAnalysis: "off");
            options.Optimizations.HasFlag(OptimizationFlags.NoOptimization).ShouldBeTrue();

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerOptions(coverageAnalysis: "gibberish");
            });
            ex.Details.ShouldBe($"Incorrect coverageAnalysis option gibberish. The options are [off, all, perTest or perTestInIsolation].");
        }

        [Fact]
        public void ShouldValidateZeroConcurrentTestrunners()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerOptions(maxConcurrentTestRunners: 0);
            });
            ex.Details.ShouldBe("Amount of maximum concurrent testrunners must be greater than zero.");
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(4, 4)]
        [InlineData(8, 8)]
        [InlineData(16, 16)]
        public void ShouldValidateConcurrentTestrunners(int given, int expected)
        {
            if(expected > Environment.ProcessorCount / 2)
            {
                // the expected concurrent testrunners should not be higher than the processor count
                expected = Environment.ProcessorCount / 2;
            }
            var options = new StrykerOptions(maxConcurrentTestRunners: given);
            options.ConcurrentTestrunners.ShouldBe(expected);
        }

        [Theory]
        [InlineData(101, "The thresholds must be between 0 and 100")]
        [InlineData(1000, "The thresholds must be between 0 and 100")]
        [InlineData(-1, "The thresholds must be between 0 and 100")]
        [InlineData(59, "The values of your thresholds are incorrect. Change `--threshold-break` to the lowest value and `--threshold-high` to the highest.")]
        public void ShouldValidateThresholdsIncorrect(int thresholdHigh, string message)
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerOptions(thresholdHigh: thresholdHigh, thresholdLow: 60, thresholdBreak: 60);
            });
            ex.Details.ShouldBe(message);
        }

        [Fact]
        public void ShouldValidateThresholds()
        {
            var options = new StrykerOptions(thresholdHigh: 60, thresholdLow: 60, thresholdBreak: 50);
            options.Thresholds.High.ShouldBe(60);
            options.Thresholds.Low.ShouldBe(60);
            options.Thresholds.Break.ShouldBe(50);
        }

        [Fact]
        public void ShouldValidateTestRunner()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerOptions(testRunner: "gibberish");
            });
            ex.Details.ShouldBe($"The given test runner (gibberish) is invalid. Valid options are: [{string.Join(",", Enum.GetValues(typeof(TestRunner)))}]");
        }
    }
}
