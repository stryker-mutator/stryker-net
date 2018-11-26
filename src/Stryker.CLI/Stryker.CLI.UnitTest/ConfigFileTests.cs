using Moq;
using Serilog.Events;
using Stryker.Core;
using Stryker.Core.Options;
using System;
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
            StrykerOptions options = new StrykerOptions("", "Console", "", 1000, "trace", false, 1, 90, 80, 70);
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
            StrykerOptions options = new StrykerOptions("", "Console", "", 1000, "trace", false, 1, 90, 80, 70);
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
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            StrykerOptions options = new StrykerOptions("", "Console", "", 1000, "trace", false, 1, 90, 80, 70);
            var runResults = new StrykerRunResult(options, 0.3M);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Returns(runResults).Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "filled-stryker-config.json" });

            mock.VerifyAll();
        }
    }
}
