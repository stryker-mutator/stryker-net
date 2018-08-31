using Moq;
using Serilog.Events;
using Stryker.Core;
using Stryker.Core.Options;
using System;
using Xunit;

namespace Stryker.CLI.UnitTest
{
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
        public void StrykerCLI_WithNoArguments_ShouldStartStrykerWithDefaultOptions()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.Is<StrykerOptions>(c => c.AdditionalTimeoutMS == 2000 &&
                                                                        c.LogOptions.LogLevel ==LogEventLevel.Information &&
                                                                        c.LogOptions.LogToFile == false &&
                                                                        c.ProjectUnderTestNameFilter == null &&
                                                                        c.Reporter == "Console"))).Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { });

            mock.VerifyAll();
        }

        [Theory]
        [InlineData("--configFilePath")]
        [InlineData("-cp")]
        public void StrykerCLI_WithConfigFile_ShouldStartStrykerWithConfigFileOptions(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.Is<StrykerOptions>(c => c.AdditionalTimeoutMS == 9999 &&
                                                                        c.LogOptions.LogLevel == LogEventLevel.Verbose &&
                                                                        c.LogOptions.LogToFile == true &&
                                                                        c.ProjectUnderTestNameFilter == "ExampleProject.csproj" &&
                                                                        c.Reporter == "RapportOnly"))).Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "filled-stryker-config.json" });

            mock.VerifyAll();
        }

        [Theory]
        [InlineData("--configFile")]
        [InlineData("-c")]
        public void StrykerCLI_WithConfigFileIsFalse_ShouldStartStrykerWithDefaultOptions(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.Is<StrykerOptions>(c => c.AdditionalTimeoutMS == 2000 &&
                                                                        c.LogOptions.LogLevel == LogEventLevel.Information &&
                                                                        c.LogOptions.LogToFile == false &&
                                                                        c.ProjectUnderTestNameFilter == null &&
                                                                        c.Reporter == "Console"))).Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "false" , "-cp", "filled-stryker-config.json"});

            mock.VerifyAll();
        }

        [Fact]
        public void StrykerCLI_OnException_ShouldExit()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Throws(new Exception("Initial testrun failed")).Verifiable();
            
            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { "-c", "false" });

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

            target.Run(new string[] { argName, "Console", "-c", "false" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.Reporter == "Console")));
        }

        [Theory]
        [InlineData("--project")]
        [InlineData("-p")]
        public void StrykerCLI_WithProjectArgument_ShouldPassProjectArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "SomeProjectName.csproj", "-c", "false" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.ProjectUnderTestNameFilter == "SomeProjectName.csproj")));
        }

        [Theory]
        [InlineData("--logConsole")]
        [InlineData("-l")]
        public void StrykerCLI_WithLogConsoleArgument_ShouldPassLogConsoleArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "debug", "-c", "false" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => 
                o.LogOptions.LogLevel == LogEventLevel.Debug && 
                o.LogOptions.LogToFile == false)));
        }

        [Theory]
        [InlineData("--logFile")]
        public void StrykerCLI_WithLogFileArgument_ShouldPassLogFileArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "true", "-c", "false" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.LogOptions.LogToFile == true)));
        }

        [Theory]
        [InlineData("--timeoutMS")]
        [InlineData("-t")]
        public void StrykerCLI_WithTimeoutArgument_ShouldPassTimeoutToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "1000", "-c", "false" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o =>
                o.AdditionalTimeoutMS == 1000)));
        }
    }
}
