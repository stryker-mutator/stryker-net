using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NuGet.Versioning;
using Serilog.Events;
using Shouldly;
using Stryker.CLI.Clients;
using Stryker.Core;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.CLI.UnitTest
{
    [Collection("StaticConfigBuilder")]
    public class StrykerCLITests
    {
        private IStrykerInputs _inputs;
        private readonly StrykerCLI _target;
        private readonly StrykerOptions _options;
        private readonly StrykerRunResult _runResults;
        private readonly Mock<IStrykerRunner> _strykerRunnerMock = new (MockBehavior.Strict);
        private readonly Mock<IStrykerNugetFeedClient> _nugetClientMock = new (MockBehavior.Strict);

        public StrykerCLITests()
        {
            _options = new StrykerOptions() { Thresholds = new Thresholds { Break = 0 } };
            _runResults = new StrykerRunResult(_options, 0.3);
            _strykerRunnerMock.Setup(x => x.RunMutationTest(It.IsAny<IStrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<IStrykerInputs, IEnumerable<LogMessage>>((c, m) => _inputs = c)
                .Returns(_runResults)
                .Verifiable();
            _nugetClientMock.Setup(x => x.GetMaxVersion()).Returns(Task.FromResult(new SemanticVersion(10, 0, 0)));
            _target = new StrykerCLI(_strykerRunnerMock.Object, _nugetClientMock.Object);
        }

        [Fact]
        public void ShouldDisplayInfoOnHelp()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var target = new StrykerCLI(mock.Object);

            using var sw = new StringWriter();
            var originalOut = Console.Out;
            try
            {
                Console.SetOut(sw);

                target.Run(new string[] { "--help" });

                string expected = @"Stryker: Stryker mutator for .Net

Stryker mutator for .Net

Usage: Stryker [options]

Options:";
                sw.ToString().ShouldStartWith(expected);
            }
            finally
            {
                //reset the console to prevent object disposed exceptions in other tests
                Console.SetOut(originalOut);
            }
        }

        [Fact]
        public void ShouldDisplayLogo()
        {
            var strykerRunnerMock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var nugetClientMock = new Mock<IStrykerNugetFeedClient>(MockBehavior.Strict);
            var strykerRunResult = new StrykerRunResult(_options, 0.3);

            strykerRunnerMock.Setup(x => x.RunMutationTest(It.IsAny<IStrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Returns(strykerRunResult)
                .Verifiable();
            nugetClientMock.Setup(x => x.GetMaxVersion()).Returns(Task.FromResult(new SemanticVersion(10, 0, 0)));

            var target = new StrykerCLI(strykerRunnerMock.Object, nugetClientMock.Object);

            using var sw = new StringWriter();
            var originalOut = Console.Out;
            try
            {
                Console.SetOut(sw);

                target.Run(new string[] { });

                string expected = @"
   _____ _              _               _   _ ______ _______  
  / ____| |            | |             | \ | |  ____|__   __| 
 | (___ | |_ _ __ _   _| | _____ _ __  |  \| | |__     | |    
  \___ \| __| '__| | | | |/ / _ \ '__| | . ` |  __|    | |    
  ____) | |_| |  | |_| |   <  __/ |    | |\  | |____   | |    
 |_____/ \__|_|   \__, |_|\_\___|_| (_)|_| \_|______|  |_|    
                   __/ |                                      
                  |___/                                       
";

                sw.ToString().ShouldContain(expected);
                sw.ToString().ShouldContain("Version:");
                sw.ToString().ShouldContain("A new version of Stryker.NET (10.0.0) is available. Please consider upgrading using `dotnet tool update -g dotnet-stryker");
            }
            finally
            {
                //reset the console to prevent object disposed exceptions in other tests
                Console.SetOut(originalOut);
            }
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

            mock.Setup(x => x.RunMutationTest(It.IsAny<IStrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Returns(strykerRunResult)
                .Verifiable();
            
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
            mock.Setup(x => x.RunMutationTest(It.IsAny<IStrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(strykerRunResult).Verifiable();

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
            mock.Setup(x => x.RunMutationTest(It.IsAny<IStrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(strykerRunResult).Verifiable();

            var target = new StrykerCLI(mock.Object, _nugetClientMock.Object);
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

            mock.Setup(x => x.RunMutationTest(It.IsAny<IStrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(strykerRunResult).Verifiable();

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
            mock.Setup(x => x.RunMutationTest(It.IsAny<IStrykerInputs>(), It.IsAny<IEnumerable<LogMessage>>())).Throws(new Exception("Initial testrun failed")).Verifiable();

            var target = new StrykerCLI(mock.Object, _nugetClientMock.Object);
            Assert.Throws<Exception>(() => target.Run(new string[] { }));
        }

        [Theory]
        [InlineData("--reporter")]
        [InlineData("-r")]
        public void ShouldPassReporterArgumentsToStryker_WithReporterArgument(string argName)
        {
            _target.Run(new string[] { argName, Reporter.Html.ToString(), argName, Reporter.Dots.ToString() });

            _strykerRunnerMock.VerifyAll();

            _inputs.ReportersInput.SuppliedInput.ShouldContain(Reporter.Html.ToString());
            _inputs.ReportersInput.SuppliedInput.ShouldContain(Reporter.Dots.ToString());
        }

        [Theory]
        [InlineData("--project")]
        [InlineData("-p")]
        public void ShouldPassProjectArgumentsToStryker_WithProjectArgument(string argName)
        {
            _target.Run(new string[] { argName, "SomeProjectName.csproj" });

            _strykerRunnerMock.VerifyAll();

            _inputs.ProjectUnderTestNameInput.SuppliedInput.ShouldBe("SomeProjectName.csproj");
        }

        [Theory]
        [InlineData("--solution")]
        [InlineData("-s")]
        public void ShouldPassSolutionArgumentPlusBasePathToStryker_WithSolutionArgument(string argName)
        {
            _target.Run(new string[] { argName, "SomeSolutionPath.sln" });

            _strykerRunnerMock.VerifyAll();

            _inputs.SolutionInput.SuppliedInput.ShouldBe("SomeSolutionPath.sln");
        }

        [Theory]
        [InlineData("--verbosity")]
        [InlineData("-V")]
        public void ShouldPassLogConsoleArgumentsToStryker_WithLogConsoleArgument(string argName)
        {
            _target.Run(new[] { argName, "Debug" });

            _strykerRunnerMock.VerifyAll();

            _inputs.VerbosityInput.SuppliedInput.ShouldBe(LogEventLevel.Debug.ToString());
        }

        [Theory]
        [InlineData("--log-to-file")]
        [InlineData("-L")]
        public void ShouldPassLogFileArgumentsToStryker_WithLogLevelFileArgument(string argName)
        {
            _target.Run(new string[] { argName });

            _strykerRunnerMock.VerifyAll();

            _inputs.LogToFileInput.SuppliedInput.Value.ShouldBeTrue();
        }

        [Theory]
        [InlineData("--dev-mode")]
        public void WithDevModeArgument_ShouldPassDevModeArgumentsToStryker(string argName)
        {
            _target.Run(new string[] { argName });

            _strykerRunnerMock.VerifyAll();

            _inputs.DevModeInput.SuppliedInput.Value.ShouldBeTrue();
        }

        [Theory]
        [InlineData("--concurrency")]
        [InlineData("-c")]
        public void WithMaxConcurrentTestrunnerArgument_ShouldPassValidatedConcurrentTestrunnersToStryker(string argName)
        {
            _target.Run(new string[] { argName, "4" });

            _strykerRunnerMock.VerifyAll();

            _inputs.ConcurrencyInput.SuppliedInput.Value.ShouldBe(4);
        }

        [Theory]
        [InlineData("--break-at")]
        [InlineData("-b")]
        public void WithCustomThresholdBreakParameter_ShouldPassThresholdBreakToStryker(string argName)
        {
            _target.Run(new string[] { argName, "20" });

            _strykerRunnerMock.VerifyAll();

            _inputs.ThresholdBreakInput.SuppliedInput.ShouldBe(20);
        }

        [Theory]
        [InlineData("--mutate")]
        [InlineData("-m")]
        public void ShouldPassFilePatternSetToStryker_WithMutateArgs(string argName)
        {
            var firstFileToExclude = "**/*Service.cs";
            var secondFileToExclude = "!**/MySpecialService.cs";
            var thirdFileToExclude = "**/MyOtherService.cs{1..10}{32..45}";

            _target.Run(new[] { argName, firstFileToExclude, argName, secondFileToExclude, argName, thirdFileToExclude });

            _strykerRunnerMock.VerifyAll();

            var filePatterns = _inputs.MutateInput.SuppliedInput.ToArray();
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
            _target.Run(new string[] { argName });

            _strykerRunnerMock.VerifyAll();

            _inputs.SinceInput.SuppliedInput.Value.ShouldBeTrue();
        }

        [Theory]
        [InlineData("--since")]
        [InlineData("-since")]
        public void ShouldSetGitDiffTargetWhenPassed(string argName)
        {
            _target.Run(new string[] { $"{argName}:development" });

            _strykerRunnerMock.VerifyAll();

            _inputs.SinceInput.SuppliedInput.Value.ShouldBeTrue();
            _inputs.SinceTargetInput.SuppliedInput.ShouldBe("development");
        }

        [Theory]
        [InlineData("--mutation-level")]
        [InlineData("-l")]
        public void ShouldSetMutationLevelWhenPassed(string argName)
        {
            _target.Run(new string[] { argName, "Advanced" });

            _inputs.MutationLevelInput.SuppliedInput.ShouldBe(MutationLevel.Advanced.ToString());
        }

        [Theory]
        [InlineData("--version", "master")]
        [InlineData("-v", "master")]
        public void ShouldEnableDiffCompareToDashboardFeatureWhenPassed(params string[] argName)
        {
            _target.Run(argName);

            _strykerRunnerMock.VerifyAll();

            _inputs.ProjectVersionInput.SuppliedInput.ShouldBe("master");
        }

        [Theory]
        [InlineData("--dashboard-api-key", "1234567890")]
        public void ShouldSupplyDashboardApiKeyWhenPassed(params string[] argName)
        {
            _target.Run(argName);

            _strykerRunnerMock.VerifyAll();

            _inputs.DashboardApiKeyInput.SuppliedInput.ShouldBe("1234567890");
        }

        [Theory]
        [InlineData("--with-baseline")]
        public void ShouldSupplyWithBaselineWhenPassed(params string[] argName)
        {
            _target.Run(argName);

            _strykerRunnerMock.VerifyAll();

            _inputs.WithBaselineInput.SuppliedInput.Value.ShouldBeTrue();
        }

        [Theory]
        [InlineData("--azure-fileshare-sas", "sas")]
        public void ShouldSupplyAzureFileshareSasWhenPassed(params string[] argName)
        {
            _target.Run(argName);

            _strykerRunnerMock.VerifyAll();

            _inputs.AzureFileStorageSasInput.SuppliedInput.ShouldBe("sas");
        }
    }
}
