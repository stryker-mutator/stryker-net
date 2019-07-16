﻿using Moq;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InitialBuildProcessTests
    {
        [Fact]
        public void InitialBuildProcess_ShouldThrowStrykerInputExceptionOnFail()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

            processMock.SetupProcessMockToReturn("", 1);

            var target = new InitialBuildProcess(processMock.Object);

            var exception = Assert.Throws<StrykerInputException>(() => target.InitialBuild(false, "/", "/"));
        }

        [Fact]
        public void InitialBuildProcess_ShouldNotThrowExceptionOnSuccess()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

            processMock.SetupProcessMockToReturn("");

            var target = new InitialBuildProcess(processMock.Object);

            target.InitialBuild(false, "/", "/");
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
    }
}
