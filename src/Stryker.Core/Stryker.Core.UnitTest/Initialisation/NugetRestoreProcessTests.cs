﻿using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Testing;
using System;
using System.IO;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class NugetRestoreProcessTests
    {
        private const string solutionPath = @"..\MySolution.sln";
        private const string CProgramFilesX86MicrosoftVisualStudio = "C:\\Program Files (x86)\\Microsoft Visual Studio";
        private readonly string solutionDir = Path.GetDirectoryName(Path.GetFullPath(solutionPath));

        [SkippableFact]
        public void HappyFlow()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "DotnetFramework does not run on Unix");

            string nugetPath = @"C:\choco\bin\NuGet.exe";
            string msBuildVersion = "16.0.0";

            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processExecutorMock.Setup(x => x.Start(solutionDir,  It.Is<string>( (p) => p.EndsWith("where.exe")), "nuget.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = nugetPath
                });

            processExecutorMock.Setup(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>( (p) => p.EndsWith("where.exe")), 
                    "-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = CProgramFilesX86MicrosoftVisualStudio
                });

            processExecutorMock.Setup(x => x.Start(solutionDir, It.IsAny<string>(), "-version /nologo", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = msBuildVersion
                });

            processExecutorMock.Setup(x => x.Start(solutionDir, nugetPath, string.Format("restore \"{0}\" -MsBuildVersion \"{1}\"", Path.GetFullPath(solutionPath), msBuildVersion), null, It.IsAny<int>()))
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

            target.RestorePackages(solutionPath);
        }

        [SkippableFact]
        public void ShouldThrowOnNugetNotInstalled()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "DotnetFramework does not run on Unix");

            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processExecutorMock.Setup(x => x.Start(solutionDir, "where.exe", "nuget.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = "INFO: Could not find files for the given pattern(s)."
                });

            var target = new NugetRestoreProcess(processExecutorMock.Object);

            var exception = Should.Throw<StrykerInputException>(() =>
            {
                target.RestorePackages(solutionPath);
            });
        }

        [SkippableFact]
        public void ShouldPickFirstNugetPath()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "DotnetFramework does not run on Unix");

            string firstNugetPath = @"C:\choco\bin\NuGet.exe";
            string msBuildVersion = "16.0.0";

            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processExecutorMock.Setup(x => x.Start(solutionDir, "where.exe", "nuget.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = $@"{firstNugetPath}
C:\Users\LEON\bin\NuGet.exe"
                });

            processExecutorMock.Setup(x => x.Start(CProgramFilesX86MicrosoftVisualStudio, It.Is<string>( (p) => p.EndsWith("where.exe")), 
                    "-latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = CProgramFilesX86MicrosoftVisualStudio
                });

            processExecutorMock.Setup(x => x.Start(solutionDir, It.IsAny<string>(), "-version /nologo", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = msBuildVersion
                });

            processExecutorMock.Setup(x => x.Start(solutionDir, firstNugetPath, string.Format("restore \"{0}\" -MsBuildVersion \"{1}\"", Path.GetFullPath(solutionPath), msBuildVersion), null, It.IsAny<int>()))
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

            target.RestorePackages(solutionPath);
        }
    }
}
