using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Core.Helpers.ProcessUtil;
using Stryker.Core.Initialisation;

namespace Stryker.Core.UnitTest.Initialisation;

[TestClass]
public class InitialBuildProcessTests : TestBase
{
    private readonly string _cProjectsExampleCsproj = Environment.OSVersion.Platform == PlatformID.Win32NT ? @"C:\Projects \Example.csproj" : "/usr/projects/Example.csproj";

    private readonly MockFileSystem _mockFileSystem = new(new Dictionary<string, MockFileData>
    {
        [@"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"] = new("msbuild code")
    });

    [TestMethod]
    public void InitialBuildProcess_ShouldThrowStrykerInputExceptionOnFail()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("", 1);

        var target = new InitialBuildProcess(processMock.Object, _mockFileSystem, TestLoggerFactory.CreateLogger<InitialBuildProcess>());

        Should.Throw<InputException>(() => target.InitialBuild(false, _cProjectsExampleCsproj, null))
            .Details.ShouldBe("Initial build of targeted project failed. Please make sure the targeted project is buildable. You can reproduce this error yourself using: \"dotnet build Example.csproj\"");
    }

    [TestMethodWithIgnoreIfSupport]
    [IgnoreIf(nameof(Is.Unix))] //DotnetFramework does not run on Unix
    public void InitialBuildProcess_WithPathAsBuildCommand_ShouldThrowStrykerInputExceptionOnFailWithQuotes()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("", 1);

        var target = new InitialBuildProcess(processMock.Object, _mockFileSystem, TestLoggerFactory.CreateLogger<InitialBuildProcess>());

        Should.Throw<InputException>(() => target.InitialBuild(true, null, _cProjectsExampleCsproj, null, targetFramework: null,
                msbuildPath: @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"))
            .Details.ShouldBe("Initial build of targeted project failed. Please make sure the targeted project is buildable. You can reproduce this error yourself using: \"\"" +
                              @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" + "\" " + Path.GetFileName(_cProjectsExampleCsproj) + "\"");
    }

    [TestMethodWithIgnoreIfSupport]
    [IgnoreIf(nameof(Is.Unix))] //DotnetFramework does not run on Unix
    public void InitialBuildProcess_WithPathAsBuildCommand_TriesWithMsBuildIfDotnetFails()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("", 2);

        var target = new InitialBuildProcess(processMock.Object, _mockFileSystem, TestLoggerFactory.CreateLogger<InitialBuildProcess>());

        Should.Throw<InputException>(() => target.InitialBuild(false, null, _cProjectsExampleCsproj, null, targetFramework: null, msbuildPath: @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"))
            .Details.ShouldBe("Initial build of targeted project failed. Please make sure the targeted project is buildable. You can reproduce this error yourself using: \"\"" +
                              @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" + "\" " + _mockFileSystem.Path.GetFileName(_cProjectsExampleCsproj) + "\"");

        processMock.Verify(x => x.Start(It.IsAny<string>(), It.Is<string>(app => app.Contains("dotnet")), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Once());
        processMock.Verify(x => x.Start(It.IsAny<string>(), It.Is<string>(app => app.Contains("MSBuild.exe")), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Exactly(2));
    }

    [TestMethod]
    public void InitialBuildProcess_ShouldNotThrowExceptionOnSuccess()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("");

        var target = new InitialBuildProcess(processMock.Object, _mockFileSystem, TestLoggerFactory.CreateLogger<InitialBuildProcess>());

        target.InitialBuild(false, "/", "/");

        processMock.Verify(p => p.Start(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Once);
    }

    // Stryker should be able to find and run MsBuild on DotnetFramework
    [TestMethodWithIgnoreIfSupport]
    [IgnoreIf(nameof(Is.Unix))] //DotnetFramework does not run on Unix
    [DataRow( @"C:\Windows\Microsoft.Net\Framework64\v2.0.50727\MSBuild.exe")]
    [DataRow( @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe")]
    [DataRow(  @"C:\Windows\Microsoft.Net\Framework\v2.0.50727\MSBuild.exe")]
    public void InitialBuildProcess_ShouldRunMsBuildOnDotnetFramework(string msBuildLocation)
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
        processMock.SetupProcessMockToReturn("");

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(msBuildLocation, new MockFileData("Mocked MsBuild Executable"));

        var target = new InitialBuildProcess(processMock.Object, mockFileSystem, TestLoggerFactory.CreateLogger<InitialBuildProcess>());

        target.InitialBuild(true, "./ExampleProject.sln", "./ExampleProject.sln", "Debug");

        processMock.Verify(x => x.Start(It.IsAny<string>(),
                It.Is<string>(applicationParam => applicationParam.Contains("msbuild.exe", StringComparison.InvariantCultureIgnoreCase)),
                It.Is<string>(argumentsParam => argumentsParam.Contains("ExampleProject.sln") && argumentsParam.Contains("/property:Configuration=Debug")),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()),
            Times.Once);

    }

    [TestMethodWithIgnoreIfSupport]
    [IgnoreIf(nameof(Is.NotWindows))] //DotnetFramework does not run on Unix
    public void InitialBuildProcess_ShouldUseCustomMsbuildIfNotNull()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
        var mockFileSystem = new MockFileSystem();
        const string CustomMsBuildPath = "C:/User/Test/Msbuild.exe";

        mockFileSystem.AddFile(CustomMsBuildPath, new MockFileData("Mocked MsBuild Executable"));

        processMock.SetupProcessMockToReturn("");

        var target = new InitialBuildProcess(processMock.Object, mockFileSystem, TestLoggerFactory.CreateLogger<InitialBuildProcess>());

        target.InitialBuild(true, "/", "./ExampleProject.sln", null, targetFramework: null, msbuildPath: CustomMsBuildPath);
        processMock.Verify(x => x.Start(It.IsAny<string>(),
                It.Is<string>(applicationParam => applicationParam == CustomMsBuildPath),
                It.Is<string>(argumentsParam => argumentsParam.Contains("ExampleProject.sln")),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()),
            Times.Once);
    }

    [TestMethod]
    public void InitialBuildProcess_ShouldUseProvidedConfiguration()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
        var mockFileSystem = new MockFileSystem();

        processMock.SetupProcessMockToReturn("");

        var target = new InitialBuildProcess(processMock.Object, mockFileSystem, TestLoggerFactory.CreateLogger<InitialBuildProcess>());

        target.InitialBuild(false, "/", "./ExampleProject.sln", "TheDebug");
        processMock.Verify(x => x.Start(It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<string>(argumentsParam => argumentsParam.Contains("-c TheDebug")),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()),
            Times.Once);
    }

    [TestMethod]
    public void InitialBuildProcess_ShouldUseProvidedPlatform()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
        var mockFileSystem = new MockFileSystem();

        processMock.SetupProcessMockToReturn("");

        var target = new InitialBuildProcess(processMock.Object, mockFileSystem, TestLoggerFactory.CreateLogger<InitialBuildProcess>());

        target.InitialBuild(false, "/", "./ExampleProject.sln", "TheDebug", "AnyCPU");
        processMock.Verify(x => x.Start(It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<string>(argumentsParam => argumentsParam.Contains("-c TheDebug")
                                                && argumentsParam.Contains("--property:Platform=AnyCPU")),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()),
            Times.Once);
    }


    [TestMethod]
    public void InitialBuildProcess_ShouldRunDotnetBuildIfNotDotnetFramework()
    {
        var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

        processMock.SetupProcessMockToReturn("");

        var target = new InitialBuildProcess(processMock.Object, _mockFileSystem, TestLoggerFactory.CreateLogger<InitialBuildProcess>());

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

        var target = new InitialBuildProcess(processMock.Object, _mockFileSystem, TestLoggerFactory.CreateLogger<InitialBuildProcess>());

        target.InitialBuild(false, "", "./ExampleProject.sln");

        processMock.Verify(x => x.Start(It.IsAny<string>(),
                It.Is<string>(applicationParam => applicationParam.Contains("dotnet", StringComparison.InvariantCultureIgnoreCase)),
                It.Is<string>(argumentsParam => argumentsParam.Contains("ExampleProject.sln")),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()),
            Times.Once);
    }
}
