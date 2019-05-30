﻿using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Testing;
using System.IO;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class NugetRestoreProcessTests
    {
        private const string solutionPath = @"..\MySolution.sln";
        private readonly string solutionDir = Path.GetDirectoryName(Path.GetFullPath(solutionPath));

        [Fact]
        public void HappyFlow()
        {
            string nugetPath = @"C:\choco\bin\NuGet.exe";
            string msBuildVersion = "16.0.0";

            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processExecutorMock.Setup(x => x.Start(solutionDir, "where.exe", "nuget.exe", null, It.IsAny<int>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = nugetPath
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

            var target = new NugetRestoreProcess(processExecutorMock.Object);

            target.RestorePackages(solutionPath);
        }

        [Fact]
        public void ShouldThrowOnNugetNotInstalled()
        {
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

        [Fact]
        public void ShouldPickFirstNugetPath()
        {
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

            var target = new NugetRestoreProcess(processExecutorMock.Object);

            target.RestorePackages(solutionPath);
        }
    }
}
