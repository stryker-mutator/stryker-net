using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Testing;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class NugetRestoreProcessTests : TestBase
    {
        private const string SolutionPath = @"..\MySolution.sln";
        private const string CProgramFilesX86MicrosoftVisualStudio = "C:\\Program Files (x86)\\Microsoft Visual Studio";
        private readonly string _solutionDir = Path.GetDirectoryName(Path.GetFullPath(SolutionPath));

        [SkippableFact]
        public void HappyFlow()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "DotnetFramework does not run on Unix");

            var nugetPath = @"C:\choco\bin\NuGet.exe";
            var msBuildVersion = "16.0.0";
            var nugetDirectory = Path.GetDirectoryName(nugetPath);

            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processExecutorMock.Setup(x => x.Start(_solutionDir, It.Is<string>((p) => p.EndsWith("where.exe")), "nuget.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = nugetPath
                });

            processExecutorMock.Setup(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>((p) => p.EndsWith("where.exe")),
                    "-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = CProgramFilesX86MicrosoftVisualStudio
                });
            processExecutorMock.Setup(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>((p) => p.EndsWith("where.exe")),
                    "-prerelease -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = CProgramFilesX86MicrosoftVisualStudio
                });

            processExecutorMock.Setup(x => x.Start(_solutionDir, It.IsAny<string>(), "-version /nologo", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = msBuildVersion
                });

            processExecutorMock.Setup(x => x.Start(nugetDirectory, nugetPath,
                    $"restore \"{Path.GetFullPath(SolutionPath)}\" -MsBuildVersion \"{msBuildVersion}\"", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = "Packages restored"
                });
            processExecutorMock.Setup(x => x.Start(It.Is<string>(s => s.Contains("Microsoft Visual Studio")), It.Is<string>(s => s.Contains("vswhere.exe")),
                @"-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\**\Bin\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = "Msbuild executable path found at "
                });
            var target = new NugetRestoreProcess(processExecutorMock.Object);

            target.RestorePackages(SolutionPath);

            processExecutorMock.Verify( p => p.Start(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Exactly(4));
        }

        [SkippableFact]
        public void ThrowIfRestoreFails()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "DotnetFramework does not run on Unix");

            var nugetPath = @"C:\choco\bin\NuGet.exe";
            var msBuildVersion = "16.0.0";
            var nugetDirectory = Path.GetDirectoryName(nugetPath);

            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processExecutorMock.Setup(x => x.Start(_solutionDir, It.Is<string>((p) => p.EndsWith("where.exe")), "nuget.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = nugetPath
                });

            processExecutorMock.Setup(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>((p) => p.EndsWith("where.exe")),
                    "-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = CProgramFilesX86MicrosoftVisualStudio
                });
            processExecutorMock.Setup(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>((p) => p.EndsWith("where.exe")),
                    "-prerelease -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = CProgramFilesX86MicrosoftVisualStudio
                });

            processExecutorMock.Setup(x => x.Start(_solutionDir, It.IsAny<string>(), "-version /nologo", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = msBuildVersion
                });

            processExecutorMock.Setup(x => x.Start(nugetDirectory, nugetPath,
                    $"restore \"{Path.GetFullPath(SolutionPath)}\" -MsBuildVersion \"{msBuildVersion}\"", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 1,
                    Output = "Packages restore failed."
                });
            processExecutorMock.Setup(x => x.Start(It.Is<string>(s => s.Contains("Microsoft Visual Studio")), It.Is<string>(s => s.Contains("vswhere.exe")),
                @"-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\**\Bin\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = "Msbuild executable path found at "
                });
            var target = new NugetRestoreProcess(processExecutorMock.Object);

            var action = () => target.RestorePackages(SolutionPath);

            action.ShouldThrow<InputException>("Packages restore failed."); 
        }

        [SkippableFact]
        public void FailToGetMsBuildVersion()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "DotnetFramework does not run on Unix");

            var nugetPath = @"C:\choco\bin\NuGet.exe";
            var msBuildVersion = string.Empty;
            var nugetDirectory = Path.GetDirectoryName(nugetPath);

            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processExecutorMock.Setup(x => x.Start(_solutionDir, It.Is<string>((p) => p.EndsWith("where.exe")), "nuget.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = nugetPath
                });

            processExecutorMock.Setup(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>((p) => p.EndsWith("where.exe")),
                    "-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = CProgramFilesX86MicrosoftVisualStudio
                });
            processExecutorMock.Setup(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>((p) => p.EndsWith("where.exe")),
                    "-prerelease -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = CProgramFilesX86MicrosoftVisualStudio
                });

            processExecutorMock.Setup(x => x.Start(_solutionDir, It.IsAny<string>(), "-version /nologo", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 1,
                    Output = msBuildVersion
                });

            processExecutorMock.Setup(x => x.Start(nugetDirectory, nugetPath,
                    $"restore \"{Path.GetFullPath(SolutionPath)}\"", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = "Packages restored"
                });
            processExecutorMock.Setup(x => x.Start(It.Is<string>(s => s.Contains("Microsoft Visual Studio")), It.Is<string>(s => s.Contains("vswhere.exe")),
                @"-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\**\Bin\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = "Msbuild executable path found at "
                });
            var target = new NugetRestoreProcess(processExecutorMock.Object);

            target.RestorePackages(SolutionPath);

            processExecutorMock.Verify( p => p.Start(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Exactly(4));
        }

        [SkippableFact]
        public void NugetIsUsingSuppliedMsBuild()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "DotnetFramework does not run on Unix");

            var nugetPath = @"C:\choco\bin\NuGet.exe";
            var msBuildVersion = "16.0.0";
            var msbuildPath = @$"{CProgramFilesX86MicrosoftVisualStudio}\2019\MSBuild\16.0\Bin\MSBuild.exe";

            var capturedMsBuildPath = "";
            var nugetDirectory = Path.GetDirectoryName(nugetPath);

            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processExecutorMock.Setup(x => x.Start(_solutionDir, It.Is<string>((p) => p.EndsWith("where.exe")), "nuget.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = nugetPath
                });

            processExecutorMock.Setup(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>((p) => p.EndsWith("where.exe")),
                    "-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = CProgramFilesX86MicrosoftVisualStudio
                });
            processExecutorMock.Setup(x => x.Start(_solutionDir, It.IsAny<string>(), "-version /nologo", null, It.IsAny<int>()))
                .Callback<string, string, string, IEnumerable<KeyValuePair<string, string>>, int>((
                    path, application, arguments, environmentVariables, timeoutMs) =>
                {
                    capturedMsBuildPath = application;
                })
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = msBuildVersion
                });

            processExecutorMock.Setup(x => x.Start(nugetDirectory, nugetPath,
                    $"restore \"{Path.GetFullPath(SolutionPath)}\" -MsBuildVersion \"{msBuildVersion}\"", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = "Packages restored"
                });

            processExecutorMock.Setup(x => x.Start(It.Is<string>(s => s.Contains("Microsoft Visual Studio")), It.Is<string>(s => s.Contains("vswhere.exe")), @"-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\**\Bin\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = "Msbuild executable path found at "
                });

            var target = new NugetRestoreProcess(processExecutorMock.Object);

            target.RestorePackages(SolutionPath, msbuildPath);
            processExecutorMock.Verify(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>((p) => p.EndsWith("where.exe")),
                "-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe", null, It.IsAny<int>()), Times.Never);
            processExecutorMock.Verify(x => x.Start(_solutionDir, It.IsAny<string>(), "-version /nologo", null, It.IsAny<int>()), Times.Once);
            processExecutorMock.Verify(x => x.Start(nugetDirectory, nugetPath, string.Format("restore \"{0}\" -MsBuildVersion \"{1}\"", Path.GetFullPath(SolutionPath), msBuildVersion), null, It.IsAny<int>()), Times.Once);
            processExecutorMock.Verify(x => x.Start(It.Is<string>(s => s.Contains("Microsoft Visual Studio")), It.Is<string>(s => s.Contains("vswhere.exe")), @"-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\**\Bin\MSBuild.exe", null, It.IsAny<int>()), Times.Never);
            Assert.Equal(msbuildPath, capturedMsBuildPath);
        }

        [SkippableFact]
        public void ShouldThrowOnNugetNotInstalled()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "DotnetFramework does not run on Unix");

            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            var msBuildVersion = "16.0.0";

            var capturedMsBuildPath = "";

            processExecutorMock.Setup(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>((p) => p.EndsWith("where.exe")),
                     It.Is<string>(s => s.EndsWith("-requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe")), null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = CProgramFilesX86MicrosoftVisualStudio
                });
            processExecutorMock.Setup(x => x.Start(_solutionDir, It.IsAny<string>(), "-version /nologo", null, It.IsAny<int>()))
                .Callback<string, string, string, IEnumerable<KeyValuePair<string, string>>, int>((
                    path, application, arguments, environmentVariables, timeoutMs) =>
                {
                    capturedMsBuildPath = application;
                })
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = msBuildVersion
                });

            processExecutorMock.Setup(x => x.Start(_solutionDir, "where.exe",  It.Is<string>( s => s.EndsWith("nuget.exe")),
                null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = "INFO: Could not find files for the given pattern(s)."
                });

            var target = new NugetRestoreProcess(processExecutorMock.Object);
            Should.Throw<InputException>(() => target.RestorePackages(SolutionPath) );
        }

        [SkippableFact]
        public void ShouldPickFirstNugetPath()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "DotnetFramework does not run on Unix");

            string firstNugetPath = @"C:\choco\bin\NuGet.exe";
            string msBuildVersion = "16.0.0";
            var nugetDirectory = Path.GetDirectoryName(firstNugetPath);

            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processExecutorMock.Setup(x => x.Start(_solutionDir, "where.exe", "nuget.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = $@"{firstNugetPath}
C:\Users\LEON\bin\NuGet.exe"
                });

            processExecutorMock.Setup(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>((p) => p.EndsWith("where.exe")),
                    "-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = CProgramFilesX86MicrosoftVisualStudio
                });

            processExecutorMock.Setup(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>((p) => p.EndsWith("where.exe")),
                    "-prerelease -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = CProgramFilesX86MicrosoftVisualStudio
                });

            processExecutorMock.Setup(x => x.Start(_solutionDir, It.IsAny<string>(), "-version /nologo", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = msBuildVersion
                });

            processExecutorMock.Setup(x => x.Start(nugetDirectory, firstNugetPath,
                    $"restore \"{Path.GetFullPath(SolutionPath)}\" -MsBuildVersion \"{msBuildVersion}\"", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = "Packages restored"
                });

            processExecutorMock.Setup(x => x.Start(It.Is<string>(s => s.Contains("Microsoft Visual Studio")), It.Is<string>(s => s.Contains("vswhere.exe")), @"-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\**\Bin\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = "Msbuild executable path found at "
                });

            var target = new NugetRestoreProcess(processExecutorMock.Object);

            target.RestorePackages(SolutionPath);
            processExecutorMock.Verify( p => p.Start(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), 0), Times.Exactly(4));
        }
    }
}
