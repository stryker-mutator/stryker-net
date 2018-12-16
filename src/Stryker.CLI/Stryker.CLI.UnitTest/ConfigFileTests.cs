using Moq;
using Serilog.Events;
using Stryker.Core;
using Stryker.Core.Options;
using System;
using System.IO;
using Xunit;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Shouldly;

namespace Stryker.CLI.UnitTest
{
    [CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
    public class ConfigFileTests
    {
        private string _currentDirectory { get; set; }

        public ConfigFileTests()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

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
            var fileToExclude = FilePathUtils.ConvertToPlatformSupportedFilePath("./Recursive.cs");
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

            actualOptions.AdditionalTimeoutMS.ShouldBe(9999);
            actualOptions.LogOptions.LogLevel.ShouldBe(LogEventLevel.Verbose);
            actualOptions.LogOptions.LogToFile.Value.ShouldBeTrue();
            actualOptions.ProjectUnderTestNameFilter.ShouldBe("ExampleProject.csproj");
            actualOptions.Reporters.ShouldHaveSingleItem();
            actualOptions.Reporters.ShouldContain(Reporter.ConsoleReport);
            actualOptions.MaxConcurrentTestrunners.ShouldBe(10);
            actualOptions.ThresholdOptions.ThresholdBreak.ShouldBe(20);
            actualOptions.ThresholdOptions.ThresholdLow.ShouldBe(30);
            actualOptions.ThresholdOptions.ThresholdHigh.ShouldBe(40);
            actualOptions.FilesToExclude.ShouldHaveSingleItem();
            actualOptions.FilesToExclude.ShouldContain(fileToExclude);
        }
    }
}
