﻿using Moq;
using Serilog.Events;
using Shouldly;
using Stryker.Core;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using System.IO;
using Xunit;

namespace Stryker.CLI.UnitTest
{
    [CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
    public class ConfigFileTests
    {
        [Fact]
        public void StrykerCLI_WithNoArgumentsAndEmptyConfig_ShouldStartStrykerWithDefaultOptions()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions options = new StrykerOptions();
            var runResults = new StrykerRunResult(options, 0.3M);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults).Verifiable();
            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { });

            mock.VerifyAll();
        }

        [Fact]
        public void StrykerCLI_WithNoArgumentsAndNoConfigFile_ShouldStartStrykerWithConfigOptions()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions options = new StrykerOptions();
            string currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory($"..{Path.DirectorySeparatorChar}");
            var runResults = new StrykerRunResult(options, 0.3M);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults).Verifiable();
            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { });

            mock.VerifyAll();

            Directory.SetCurrentDirectory(currentDirectory);
        }

        [Theory]
        [InlineData("--config-file-path")]
        [InlineData("-cp")]
        public void StrykerCLI_WithConfigFile_ShouldStartStrykerWithConfigFileOptions(string argName)
        {
            var fileToExclude = FilePathUtils.ConvertPathSeparators("./Recursive.cs");
            StrykerOptions actualOptions = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.3M);

            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()))
                .Callback<StrykerOptions>((c) => actualOptions = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "filled-stryker-config.json" });

            mock.VerifyAll();

            actualOptions.DevMode.ShouldBe(true);
            actualOptions.AdditionalTimeoutMS.ShouldBe(9999);
            actualOptions.LogOptions.LogLevel.ShouldBe(LogEventLevel.Verbose);
            actualOptions.ProjectUnderTestNameFilter.ShouldBe("ExampleProject.csproj");
            actualOptions.Reporters.ShouldHaveSingleItem();
            actualOptions.Reporters.ShouldContain(Reporter.ConsoleReport);
            actualOptions.ConcurrentTestrunners.ShouldBe(1);
            actualOptions.Thresholds.Break.ShouldBe(20);
            actualOptions.Thresholds.Low.ShouldBe(30);
            actualOptions.Thresholds.High.ShouldBe(40);
            actualOptions.FilesToExclude.ShouldHaveSingleItem();
            actualOptions.FilesToExclude.ShouldContain(fileToExclude);
            actualOptions.Optimizations.ShouldBe(OptimizationFlags.CoverageBasedTest | OptimizationFlags.AbortTestOnKill);
        }
    }
}
