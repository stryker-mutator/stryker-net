using System;
using System.Collections.Generic;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Testing;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InitialBuildProcessTests : TestBase
    {
        [Fact]
        public void InitialBuildProcess_ShouldThrowStrykerInputExceptionOnFail()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

            processMock.SetupProcessMockToReturn("", 1);

            var target = new InitialBuildProcess(processMock.Object);

            Should.Throw<InputException>(() => target.InitialBuild(false, @"C:\Projects\Example.csproj", null))
                .Details.ShouldBe("Initial build of targeted project failed. Please make sure the targeted project is buildable. You can reproduce this error yourself using: \"dotnet build \"" + @"C:\Projects\Example.csproj" + "\"\"");
        }

        [SkippableFact]
        public void InitialBuildProcess_WithPathAsBuildCommand_ShouldThrowStrykerInputExceptionOnFailWithQuotes()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "MSBuild is only available on Windows");

            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

            processMock.SetupProcessMockToReturn("", 1);

            var target = new InitialBuildProcess(processMock.Object);

            Should.Throw<InputException>(() => target.InitialBuild(true, null, @"C:\Projects\Example.csproj", @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"))
                .Details.ShouldBe("Initial build of targeted project failed. Please make sure the targeted project is buildable. You can reproduce this error yourself using: \"\"" +
                                  @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" + "\" \"" + @"C:\Projects\Example.csproj" + "\"\"");
        }

        [Fact]
        public void InitialBuildProcess_ShouldNotThrowExceptionOnSuccess()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

            processMock.SetupProcessMockToReturn("");

            var target = new InitialBuildProcess(processMock.Object);

            target.InitialBuild(false, "/", "/");

            processMock.Verify( p => p.Start(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Once);
        }

        [SkippableFact]
        public void InitialBuildProcess_ShouldRunMsBuildOnDotnetFramework()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "DotnetFramework does not run on Unix");

            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

            processMock.SetupProcessMockToReturn("");

            var target = new InitialBuildProcess(processMock.Object);

            target.InitialBuild(true, "/", "./ExampleProject.sln");

            processMock.Verify(x => x.Start(It.IsAny<string>(),
                It.Is<string>(applicationParam => applicationParam.Contains("msbuild.exe", StringComparison.InvariantCultureIgnoreCase)),
                It.Is<string>(argumentsParam => argumentsParam.Contains("ExampleProject.sln")),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()),
                Times.Once);
        }

        [Fact]
        public void InitialBuildProcess_ShouldUseCustomMsbuildIfNotNull()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

            processMock.SetupProcessMockToReturn("");

            var target = new InitialBuildProcess(processMock.Object);

            target.InitialBuild(true, "/", "./ExampleProject.sln", "C:/User/Test/Msbuild.exe");

            processMock.Verify(x => x.Start(It.IsAny<string>(),
                It.Is<string>(applicationParam => applicationParam.Contains("C:/User/Test/Msbuild.exe", StringComparison.InvariantCultureIgnoreCase)),
                It.Is<string>(argumentsParam => argumentsParam.Contains("ExampleProject.sln")),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()),
                Times.Once);
        }

        [Fact]
        public void InitialBuildProcess_ShouldRunDotnetBuildIfNotDotnetFramework()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

            processMock.SetupProcessMockToReturn("");

            var target = new InitialBuildProcess(processMock.Object);

            target.InitialBuild(false, "./ExampleProject.csproj", null);

            processMock.Verify(x => x.Start(It.IsAny<string>(),
                It.Is<string>(applicationParam => applicationParam.Contains("dotnet", StringComparison.InvariantCultureIgnoreCase)),
                It.Is<string>(argumentsParam => argumentsParam.Contains("build")),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()),
                Times.Once);
        }

        [Fact]
        public void InitialBuildProcess_ShouldUseSolutionPathIfSet()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

            processMock.SetupProcessMockToReturn("");

            var target = new InitialBuildProcess(processMock.Object);

            target.InitialBuild(false, "", "./ExampleProject.sln");

            processMock.Verify(x => x.Start(It.IsAny<string>(),
                It.Is<string>(applicationParam => applicationParam.Contains("dotnet", StringComparison.InvariantCultureIgnoreCase)),
                It.Is<string>(argumentsParam => argumentsParam.Contains("ExampleProject.sln")),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()),
                Times.Once);
        }
    }
}
