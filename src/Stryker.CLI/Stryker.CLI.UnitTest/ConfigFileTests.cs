using DotNet.Globbing;
using Microsoft.CodeAnalysis.Text;
using Moq;
using Serilog.Events;
using Shouldly;
using Stryker.Core;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Stryker.CLI.UnitTest
{
    [CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
    public class ConfigFileTests
    {
        [Fact]
        public void StrykerCLI_WithNoArgumentsAndEmptyConfig_ShouldStartStrykerWithDefaultOptions()
        {
            // Arrange
            string currentDirectory = Directory.GetCurrentDirectory();
            var strykerRunner = new Mock<IStrykerRunner>(MockBehavior.Strict);
            strykerRunner.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(It.IsAny<StrykerRunResult>()).Verifiable();

            // Act
            new StrykerCLI(strykerRunner.Object).Run(new string[] { });

            // Assert
            strykerRunner.VerifyAll();
        }

        [Fact]
        public void StrykerCLI_WithNoArgumentsAndNoConfigFile_ShouldStartStrykerWithConfigOptions()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var options = new StrykerOptions();
            string currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory($"..{Path.DirectorySeparatorChar}");
            var runResults = new StrykerRunResult(options, 0.3);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>())).Returns(runResults).Verifiable();
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
            var filePattern = new FilePattern(Glob.Parse(FilePathUtils.NormalizePathSeparators("**/Test.cs")), true, new[] { TextSpan.FromBounds(1, 100), TextSpan.FromBounds(200, 300) });
            StrykerOptions actualOptions = null;
            var runResults = new StrykerRunResult(new StrykerOptions(), 0.3);

            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<LogMessage>>()))
                .Callback<StrykerOptions, IEnumerable<LogMessage>>((c, m) => actualOptions = c)
                .Returns(runResults)
                .Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "filled-stryker-config.json" });

            mock.VerifyAll();

            actualOptions.DevMode.ShouldBe(true);
            actualOptions.AdditionalTimeoutMS.ShouldBe(9999);
            actualOptions.LogOptions.LogLevel.ShouldBe(LogEventLevel.Verbose);
            actualOptions.ProjectUnderTestName.ShouldBe("ExampleProject.csproj");
            actualOptions.Reporters.ShouldHaveSingleItem();
            actualOptions.Reporters.ShouldContain(Reporter.ConsoleReport);
            actualOptions.Concurrency.ShouldBe(1);
            actualOptions.Thresholds.Break.ShouldBe(20);
            actualOptions.Thresholds.Low.ShouldBe(30);
            actualOptions.Thresholds.High.ShouldBe(40);
            actualOptions.Mutate.Count().ShouldBe(2);
            actualOptions.Mutate.ShouldContain(filePattern);
            actualOptions.OptimizationMode.ShouldBe(OptimizationModes.CoverageBasedTest | OptimizationModes.DisableAbortTestOnKill);
        }
    }
}
