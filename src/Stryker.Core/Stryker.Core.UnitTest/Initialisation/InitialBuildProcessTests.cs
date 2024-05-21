using System;
using System.Collections.Generic;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Testing;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation;

public class InitialBuildProcessTests : TestBase
{
    private readonly string _cProjectsExampleCsproj;

    public InitialBuildProcessTests() => _cProjectsExampleCsproj = Environment.OSVersion.Platform == PlatformID.Win32NT ? @"C:\Projects \Example.csproj" : "/usr/projects/Example.csproj";

    [Fact]
    public void InitialBuildProcess_ShouldThrowStrykerInputExceptionOnFail()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("", 1);

        var target = new InitialBuildProcess(processMock.Object);

        Should.Throw<InputException>(() => target.InitialBuild(false, _cProjectsExampleCsproj, null))
            .Details.ShouldBe("Initial build of targeted project failed. Please make sure the targeted project is buildable. You can reproduce this error yourself using: \"dotnet build Example.csproj\"");
    }

    [SkippableFact]
    public void InitialBuildProcess_WithPathAsBuildCommand_ShouldThrowStrykerInputExceptionOnFailWithQuotes()
    {
        Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "MSBuild is only available on Windows");

        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("", 1);

        var target = new InitialBuildProcess(processMock.Object);

        Should.Throw<InputException>(() => target.InitialBuild(true, null, _cProjectsExampleCsproj, null, @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"))
            .Details.ShouldBe("Initial build of targeted project failed. Please make sure the targeted project is buildable. You can reproduce this error yourself using: \"\"" +
                              @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" + "\" \"" + _cProjectsExampleCsproj + "\"\"");
    }

    [SkippableFact]
    public void InitialBuildProcess_WithPathAsBuildCommand_TriesWithMsBuildIfDotnetFails()
    {
        Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "MSBuild is only available on Windows");

        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("", 2);

        var target = new InitialBuildProcess(processMock.Object);

        Should.Throw<InputException>(() => target.InitialBuild(false, null, _cProjectsExampleCsproj, null, @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"))
            .Details.ShouldBe("Initial build of targeted project failed. Please make sure the targeted project is buildable. You can reproduce this error yourself using: \"\"" +
                              @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" + "\" \"" + _cProjectsExampleCsproj + "\"\"");

        processMock.Verify(x =>x.Start(It.IsAny<string>(), It.Is<string>(app => app.Contains("dotnet")), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Once());
        processMock.Verify(x =>x.Start(It.IsAny<string>(), It.Is<string>(app => app.Contains("MSBuild.exe")), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Exactly(3));
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

    [Fact]
    public void InitialBuildProcess_ShouldUseConfigurationWhenProvided()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("");

        var target = new InitialBuildProcess(processMock.Object);

        target.InitialBuild(false, "/", "/", "Release");

        processMock.Verify( p => p.Start(It.IsAny<string>(), It.IsAny<string>(), It.Is<string>(x => x.Contains("Release")),
            It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Once);
    }

    [SkippableFact]
    public void InitialBuildProcess_ShouldRunMsBuildOnDotnetFramework()
    {
        Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "DotnetFramework does not run on Unix");

        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("");

        var target = new InitialBuildProcess(processMock.Object);

        target.InitialBuild(true, "./ExampleProject.sln", "./ExampleProject.sln", "Debug");

        processMock.Verify(x => x.Start(It.IsAny<string>(),
                It.Is<string>(applicationParam => applicationParam.Contains("msbuild.exe", StringComparison.InvariantCultureIgnoreCase)),
                It.Is<string>(argumentsParam => argumentsParam.Contains("ExampleProject.sln") && argumentsParam.Contains("/property:Configuration=Debug")),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()),
            Times.Once);

    }

    [SkippableFact]
    public void InitialBuildProcess_ShouldRequireSolutionWhenRunMsBuild()
    {
        Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "DotnetFramework does not run on Unix");

        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("");

        var target = new InitialBuildProcess(processMock.Object);

        target.InitialBuild(true, "./ExampleProject.sln", "./ExampleProject.sln");

        var action= () => target.InitialBuild(true, "./ExampleProject.sln", null);
        action.ShouldThrow<InputException>().Message.ShouldBe("Stryker could not build your project as no solution file was presented. Please pass the solution path to stryker.");
    }

    [Fact]
    public void InitialBuildProcess_ShouldUseCustomMsbuildIfNotNull()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("");

        var target = new InitialBuildProcess(processMock.Object);

        var customMsBuildPath = "C:/User/Test/Msbuild.exe";
        target.InitialBuild(true, "/", "./ExampleProject.sln", null, customMsBuildPath);
        var executable =Environment.OSVersion.Platform == PlatformID.Win32NT ? customMsBuildPath : "dotnet";
        processMock.Verify(x => x.Start(It.IsAny<string>(),
                It.Is<string>(applicationParam => applicationParam == executable),
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
