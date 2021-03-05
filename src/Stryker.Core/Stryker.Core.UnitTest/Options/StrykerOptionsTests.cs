using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Serilog.Events;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

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
        public void ShouldValidateLoglevel()
        {
            var logLevel = "incorrect";

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerOptions(logLevel: logLevel);
            });

            ex.Message.ShouldBe("The value for one of your settings is not correct. Try correcting or removing them.");
            ex.Details.ShouldBe($"Incorrect log level ({logLevel}). The log level options are [Verbose, Debug, Information, Warning, Error, Fatal]");
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

        [Theory]
        [InlineData("./MyFolder/MyFile.cs", "MyFolder/MyFile.cs")]
        [InlineData("./MyFolder", "MyFolder/*.*")]
        [InlineData("C:/MyFolder/MyFile.cs", "C:/MyFolder/MyFile.cs")]
        public void FilesToExclude_should_be_converted_to_file_patterns(string fileToExclude, string expectedFilePattern)
        {
            // Act
            var result = new StrykerOptions(filesToExclude: new[] { fileToExclude });

            // Assert
            var pattern = result.FilePatterns.Last();
            Path.GetFullPath(pattern.Glob.ToString()).ShouldBe(Path.GetFullPath(expectedFilePattern));
            pattern.TextSpans.ShouldContain(TextSpan.FromBounds(0, int.MaxValue));
            pattern.IsExclude.ShouldBeTrue();
        }

        [Fact]
        public void ShouldValidateApiKey()
        {
            const string strykerDashboardApiKey = "STRYKER_DASHBOARD_API_KEY";
            var key = Environment.GetEnvironmentVariable(strykerDashboardApiKey);
            try
            {
                var options = new StrykerOptions();
                Environment.SetEnvironmentVariable(strykerDashboardApiKey, string.Empty);

                var ex = Assert.Throws<StrykerInputException>(() =>
                {
                    new StrykerOptions(reporters: new string[] { "Dashboard" });
                });
                ex.Message.ShouldContain($"An API key is required when the {Reporter.Dashboard} reporter is turned on! You can get an API key at {options.DashboardUrl}");
                ex.Message.ShouldContain($"A project name is required when the {Reporter.Dashboard} reporter is turned on!");
            }
            finally
            {
                Environment.SetEnvironmentVariable(strykerDashboardApiKey, key);
            }
        }

        [Fact]
        public void ShouldValidateGitSource()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerOptions(gitDiffTarget: "");
            });
            ex.Message.ShouldBe("GitDiffTarget may not be empty, please provide a valid git branch name");
        }

        [Fact]
        public void ShouldValidateExcludedMutation()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerOptions(excludedMutations: new[] { "gibberish" });
            });
            ex.Details.ShouldBe($"Invalid excluded mutation (gibberish). The excluded mutations options are [Arithmetic, Equality, Boolean, Logical, Assignment, Unary, Update, Checked, Linq, String, Bitwise, Initializer, Regex]");
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
            ex.Details.ShouldBe($"Incorrect coverageAnalysis option (gibberish). The options are [Off, All, PerTest or PerTestInIsolation].");
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
        public void ShouldValidateMutationLevel()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerOptions(mutationLevel: "gibberish");
            });
            ex.Details.ShouldBe($"The given mutation level (gibberish) is invalid. Valid options are: [Basic, Standard, Advanced, Complete]");
        }

        [Fact]
        public void ShouldValidateLanguageVersion()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerOptions(languageVersion: "gibberish");
            });
            ex.Details.ShouldBe($"The given c# language version (gibberish) is invalid. Valid options are: [{string.Join(", ", ((IEnumerable<LanguageVersion>)Enum.GetValues(typeof(LanguageVersion))).Where(l => l != LanguageVersion.CSharp1))}]");
        }

        [Fact]
        public void ShouldValidateCoverageAnalysis()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerOptions(coverageAnalysis: "gibberish");
            });
            ex.Details.ShouldBe($"Incorrect coverageAnalysis option (gibberish). The options are [Off, All, PerTest or PerTestInIsolation].");
        }

        [Fact]
        public void ShouldValidateTestRunner()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerOptions(testRunner: "gibberish");
            });
            ex.Details.ShouldBe($"The given test runner (gibberish) is invalid. Valid options are: [VsTest, DotnetTest]");
        }

        [Fact]
        public void ProjectVersionCannotBeEmpty()
        {
            static void act() => new StrykerOptions(reporters: new[] { "dashboard" }, compareToDashboard: true, projectVersion: string.Empty, projectName: "test", dashboardApiKey: "someKey");

            Should.Throw<StrykerInputException>(act)
                .Message.ShouldBe("When the compare to dashboard feature is enabled, dashboard-version cannot be empty, please provide a dashboard-version");
        }

        [Fact]
        public void ProjectVersionCannotBeNull()
        {
            static void act() => new StrykerOptions(reporters: new[] { "dashboard" }, compareToDashboard: true, projectVersion: null, fallbackVersion: "fallbackVersion", projectName: "test", dashboardApiKey: "someKey");

            Should.Throw<StrykerInputException>(act)
                .Message.ShouldBe("When the compare to dashboard feature is enabled, dashboard-version cannot be empty, please provide a dashboard-version");
        }

        [Fact]
        public void FallbackVersionCannotBeProjectVersion()
        {
            static void act() => new StrykerOptions(reporters: new[] { "dashboard" }, compareToDashboard: true, projectVersion: "version", fallbackVersion: "version", projectName: "test", dashboardApiKey: "someKey");

            Should.Throw<StrykerInputException>(act)
                .Message.ShouldBe("Fallback version cannot be set to the same value as the dashboard-version, please provide a different fallback version");
        }

        [Fact]
        public void ShouldNotThrowInputExceptionWhenSetCorrectly()
        {
            static void act() => new StrykerOptions(reporters: new[] { "dashboard" }, compareToDashboard: true, projectVersion: "version", fallbackVersion: "fallbackVersion", projectName: "test", dashboardApiKey: "someKey");

            Should.NotThrow(act);
        }

        [Fact]
        public void ShouldSetFallbackToGitSourceWhenNullAndCompareEnabled()
        {
            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version", fallbackVersion: null, gitDiffTarget: "development");

            options.GitDiffSource.ShouldBe("development");
            options.FallbackVersion.ShouldBe("development");
        }

        [Fact]
        public void Should_Throw_Exception_When_AzureSAS_null()
        {
            static void act() => new StrykerOptions(azureFileStorageUrl: "https://www.example.com", azureSAS: null, baselineStorageLocation: "AzureFileStorage");

            Should.Throw<StrykerInputException>(act).Message.ShouldBe("A Shared Access Signature is required when Azure File Storage is enabled!");
        }

        [Fact]
        public void Should_Throw_Exception_When_Azure_Storage_url_null()
        {
            static void act() => new StrykerOptions(azureFileStorageUrl: null, azureSAS: "AZURE_SAS", baselineStorageLocation: "AzureFileStorage");

            Should.Throw<StrykerInputException>(act).Message.ShouldBe("The url pointing to your file storage is required when Azure File Storage is enabled!");
        }

        [Fact]
        public void Should_Throw_Exception_When_Azure_Storage_url_and_SAS_null()
        {
            static void act() => new StrykerOptions(azureFileStorageUrl: null, azureSAS: null, baselineStorageLocation: "AzureFileStorage");

            Should.Throw<StrykerInputException>(act).Message.ShouldBe(@"A Shared Access Signature is required when Azure File Storage is enabled!The url pointing to your file storage is required when Azure File Storage is enabled!");
        }

        [Fact]
        public void Should_Normalize_SAS()
        {
            var target = new StrykerOptions(azureFileStorageUrl: "https://www.example.com", azureSAS: "?sv=SAS", baselineStorageLocation: "AzureFileStorage");

            target.AzureSAS.ShouldBe("SAS");
        }

        [Fact]
        public void Should_Enabled_Diff_When_CompareToDashboard_Is_Enabled()
        {
            var target = new StrykerOptions(compareToDashboard: true, projectVersion: "version", fallbackVersion: "fallbackVersion", projectName: "test", dashboardApiKey: "someKey");

            target.DiffEnabled.ShouldBeTrue();
        }
    }
}
