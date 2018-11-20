using Moq;
using Serilog.Events;
using Stryker.Core;
using Stryker.Core.Options;
using System;
using System.IO;
using Xunit;
using System.Reflection;

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
            mock.Setup(x => x.RunMutationTest(It.Is<StrykerOptions>(c => c.AdditionalTimeoutMS == 30000 &&
                                                                        c.LogOptions.LogLevel == LogEventLevel.Information &&
                                                                        c.LogOptions.LogToFile == false &&
                                                                        c.ProjectUnderTestNameFilter == null &&
                                                                        c.Reporter == "Console" && 
                                                                        c.MaxConcurrentTestrunners == int.MaxValue &&
                                                                        c.ThresholdOptions.ThresholdBreak == 0 &&
                                                                        c.ThresholdOptions.ThresholdLow == 60 &&
                                                                        c.ThresholdOptions.ThresholdHigh == 80))).Verifiable();
            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { });

            mock.VerifyAll();
        }

        [Fact]
        public void StrykerCLI_WithNoArgumentsAndNoConfigFile_ShouldStartStrykerWithConfigOptions()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.Is<StrykerOptions>(c => c.AdditionalTimeoutMS == 30000 &&
                                                                        c.LogOptions.LogLevel == LogEventLevel.Information &&
                                                                        c.LogOptions.LogToFile == false &&
                                                                        c.ProjectUnderTestNameFilter == null &&
                                                                        c.Reporter == "Console" &&
                                                                        c.MaxConcurrentTestrunners == int.MaxValue &&
                                                                        c.ThresholdOptions.ThresholdBreak == 0 &&
                                                                        c.ThresholdOptions.ThresholdLow == 60 &&
                                                                        c.ThresholdOptions.ThresholdHigh == 80))).Verifiable();
            string currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory($"..{Path.DirectorySeparatorChar}");
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

            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.Is<StrykerOptions>(c => c.AdditionalTimeoutMS == 9999 &&
                                                                        c.LogOptions.LogLevel == LogEventLevel.Verbose &&
                                                                        c.LogOptions.LogToFile == true &&
                                                                        c.ProjectUnderTestNameFilter == "ExampleProject.csproj" &&
                                                                        c.Reporter == "ReportOnly" &&
                                                                        c.MaxConcurrentTestrunners == 10 &&
                                                                        c.ThresholdOptions.ThresholdBreak == 20 &&
                                                                        c.ThresholdOptions.ThresholdLow == 30 &&
                                                                        c.ThresholdOptions.ThresholdHigh == 40 && 
                                                                        c.FilesToExclude[0] == fileToExclude))).Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "filled-stryker-config.json" });

            mock.VerifyAll();
        }
    }
}
