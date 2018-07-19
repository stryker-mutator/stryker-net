﻿using Moq;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    public class DotnetTestRunnerTests
    {
        [Fact]
        public void RunAllExitCode0SuccessShouldBeTrue()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.SetupProcessMockToReturn("Testrun successful");

            string path = "/test";
            var target = new DotnetTestRunner(path, processMock.Object);

            var result = target.RunAll(null);

            Assert.True(result.Success);
            processMock.Verify(m => m.Start(path, "dotnet", It.Is<string>(s => s.Contains("test")), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<int>()));
        }

        [Fact]
        public void RunAllExitCode1SuccessShouldBeFalse()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.SetupProcessMockToReturn("Testrun failed", 1);

            string path = "/test";
            var target = new DotnetTestRunner(path, processMock.Object);

            var result = target.RunAll(null);

            Assert.False(result.Success);
            processMock.Verify(m => m.Start(path, "dotnet", It.Is<string>(s => s.Contains("test")), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<int>()));
        }

        [Fact]
        public void RunAllExceptionStatusCodeSuccessShouldBeFalse()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.SetupProcessMockToReturn("Testrun failed other way", -100);

            string path = "/test";
            var target = new DotnetTestRunner(path, processMock.Object);

            var result = target.RunAll(null);

            Assert.False(result.Success);
            processMock.Verify(m => m.Start(path, "dotnet", It.Is<string>(s => s.Contains("test")), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<int>()));
        }

        [Fact]
        public void EnvironmentVariableGetsPassed()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.SetupProcessMockToReturn("Testrun failed other way", -100);

            string path = "/test";
            var target = new DotnetTestRunner(path, processMock.Object);

            target.SetActiveMutation(1);
            var result = target.RunAll(null);

            Assert.False(result.Success);
            processMock.Verify(m => m.Start(
                path, 
                "dotnet", 
                It.Is<string>(s => s.Contains("test")), 
                It.Is<IEnumerable<KeyValuePair<string, string>>>(
                    x => x.Where(y => y.Value == "1" && y.Key == "ActiveMutation").Any()
                ),
                It.IsAny<int>()));
        }
    }
}
