using Moq;
using Serilog.Events;
using Stryker.Core;
using Stryker.Core.Options;
using System;
using System.IO;
using Xunit;

namespace Stryker.CLI.UnitTest
{
    using System.Linq;

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
        public void StrykerCLI_OnException_ShouldExit()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Throws(new Exception("Initial testrun failed")).Verifiable();
            
            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { });

            mock.VerifyAll();
        }

        [Theory]
        [InlineData("--reporter")]
        [InlineData("-r")]
        public void StrykerCLI_WithReporterArgument_ShouldPassReporterArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "Console" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.Reporter == "Console")));
        }

        [Theory]
        [InlineData("--project-file")]
        [InlineData("-p")]
        public void StrykerCLI_WithProjectArgument_ShouldPassProjectArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "SomeProjectName.csproj" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.ProjectUnderTestNameFilter == "SomeProjectName.csproj")));
        }

        [Theory]
        [InlineData("--log-console")]
        [InlineData("-l")]
        public void StrykerCLI_WithLogConsoleArgument_ShouldPassLogConsoleArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new[] { argName, "debug" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => 
                o.LogOptions.LogLevel == LogEventLevel.Debug && 
                o.LogOptions.LogToFile == false)));
        }

        [Theory]
        [InlineData("--log-level-file")]
        public void StrykerCLI_WithLogLevelFileArgument_ShouldPassLogFileArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "true" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.LogOptions.LogToFile == true)));
        }

        [Theory]
        [InlineData("--timeout-ms")]
        [InlineData("-t")]
        public void StrykerCLI_WithTimeoutArgument_ShouldPassTimeoutToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "1000" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.AdditionalTimeoutMS == 1000)));
        }

        [Theory]
        [InlineData("--max-concurrent-test-runners")]
        [InlineData("-m")]
        public void StrykerCLI_WithMaxConcurrentTestrunnerArgument_ShouldPassMaxConcurrentTestrunnerToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);
            
            target.Run(new string[] { argName, "4" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.MaxConcurrentTestrunners == 4)));
        }

        [Theory]
        [InlineData("--threshold-break")]
        [InlineData("-tb")]
        public void StrykerCLI_WithCustomThresholdBreakParameter_ShouldPassThresholdBreakToStryker(string argName) 
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "20" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.ThresholdOptions.ThresholdBreak == 20)));
        }

        [Theory]
        [InlineData("--threshold-low")]
        [InlineData("-tl")]
        public void StrykerCLI_WithCustomThresholdLowParameter_ShouldPassThresholdLowToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "65" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.ThresholdOptions.ThresholdLow == 65)));
        }

        [Theory]
        [InlineData("--threshold-high")]
        [InlineData("-th")]
        public void StrykerCLI_WithCustomThresholdHighParameter_ShouldPassThresholdHighToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "90" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.ThresholdOptions.ThresholdHigh == 90)));
        }

        [Fact]
        public void StrykerCLI_WithNoFilesToExcludeSet_ShouldPassDefaultValueToStryker()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

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
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new[] { argName, "['/StartUp.cs']" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.FilesToExclude[0] == "/StartUp.cs")));
        }
    }
}
