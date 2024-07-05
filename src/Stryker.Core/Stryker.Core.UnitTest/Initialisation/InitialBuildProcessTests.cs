using System;
using System.Collections.Generic;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Initialisation;

[TestClass]
public class InitialBuildProcessTests : TestBase
{
    private readonly string _cProjectsExampleCsproj;

    public InitialBuildProcessTests() => _cProjectsExampleCsproj = Environment.OSVersion.Platform == PlatformID.Win32NT ? @"C:\Projects \Example.csproj" : "/usr/projects/Example.csproj";

    [TestMethod]
    public void InitialBuildProcess_ShouldThrowStrykerInputExceptionOnFail()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("", 1);

        var target = new InitialBuildProcess(processMock.Object);

        Should.Throw<InputException>(() => target.InitialBuild(false, _cProjectsExampleCsproj, null))
            .Details.ShouldBe("Initial build of targeted project failed. Please make sure the targeted project is buildable. You can reproduce this error yourself using: \"dotnet build Example.csproj\"");
    }

    [TestMethodWithIgnoreIfSupport]
    [IgnoreIf(nameof(Is.Unix))] //DotnetFramework does not run on Unix
    public void InitialBuildProcess_WithPathAsBuildCommand_ShouldThrowStrykerInputExceptionOnFailWithQuotes()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("", 1);

        var target = new InitialBuildProcess(processMock.Object);

        Should.Throw<InputException>(() => target.InitialBuild(true, null, _cProjectsExampleCsproj, null, @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"))
            .Details.ShouldBe("Initial build of targeted project failed. Please make sure the targeted project is buildable. You can reproduce this error yourself using: \"\"" +
                              @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" + "\" \"" + _cProjectsExampleCsproj + "\"\"");
    }

    [TestMethodWithIgnoreIfSupport]
    [IgnoreIf(nameof(Is.Unix))] //DotnetFramework does not run on Unix
    public void InitialBuildProcess_WithPathAsBuildCommand_TriesWithMsBuildIfDotnetFails()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("", 2);

        var target = new InitialBuildProcess(processMock.Object);

        Should.Throw<InputException>(() => target.InitialBuild(false, null, _cProjectsExampleCsproj, null, @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"))
            .Details.ShouldBe("Initial build of targeted project failed. Please make sure the targeted project is buildable. You can reproduce this error yourself using: \"\"" +
                              @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" + "\" \"" + _cProjectsExampleCsproj + "\"\"");

        processMock.Verify(x => x.Start(It.IsAny<string>(), It.Is<string>(app => app.Contains("dotnet")), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Once());
        processMock.Verify(x =>x.Start(It.IsAny<string>(), It.Is<string>(app => app.Contains("MSBuild.exe")), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Exactly(2));
    }

    [TestMethod]
    public void InitialBuildProcess_ShouldNotThrowExceptionOnSuccess()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("");

        var target = new InitialBuildProcess(processMock.Object);

        target.InitialBuild(false, "/", "/");

        processMock.Verify(p => p.Start(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Once);
    }

    [TestMethodWithIgnoreIfSupport]
    [IgnoreIf(nameof(Is.Unix))] //DotnetFramework does not run on Unix
    public void InitialBuildProcess_ShouldRunMsBuildOnDotnetFramework()
    {
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

    [TestMethod]
    public void InitialBuildProcess_ShouldUseCustomMsbuildIfNotNull()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("");

        var target = new InitialBuildProcess(processMock.Object);

        var customMsBuildPath = "C:/User/Test/Msbuild.exe";
        target.InitialBuild(true, "/", "./ExampleProject.sln", null, customMsBuildPath);
        processMock.Verify(x => x.Start(It.IsAny<string>(),
                It.Is<string>(applicationParam => applicationParam == customMsBuildPath),
                It.Is<string>(argumentsParam => argumentsParam.Contains("ExampleProject.sln")),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()),
            Times.Once);
    }

    [TestMethod]
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

    [TestMethod]
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
