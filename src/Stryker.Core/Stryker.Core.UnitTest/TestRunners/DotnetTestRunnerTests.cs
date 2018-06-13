using Moq;
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
            processMock.Setup(x => x.Start(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()))
                .Returns(new ProcessResult() { ExitCode = 0 });
            string path = "/test";
            var target = new DotnetTestRunner(path, processMock.Object);

            var result = target.RunAll();

            Assert.True(result.Success);
            processMock.Verify(m => m.Start(path, "dotnet", It.Is<string>(s => s.Contains("test")), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<int>()));
        }

        [Fact]
        public void RunAllExitCode1SuccessShouldBeFalse()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.Setup(x => x.Start(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()))
                .Returns(new ProcessResult() { ExitCode = 1 });
            string path = "/test";
            var target = new DotnetTestRunner(path, processMock.Object);

            var result = target.RunAll();

            Assert.False(result.Success);
            processMock.Verify(m => m.Start(path, "dotnet", It.Is<string>(s => s.Contains("test")), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<int>()));
        }

        [Fact]
        public void RunAllExceptionStatusCodeSuccessShouldBeFalse()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.Setup(x => x.Start(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()))
                .Returns(new ProcessResult() { ExitCode = -100 });
            string path = "/test";
            var target = new DotnetTestRunner(path, processMock.Object);

            var result = target.RunAll();

            Assert.False(result.Success);
            processMock.Verify(m => m.Start(path, "dotnet", It.Is<string>(s => s.Contains("test")), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<int>()));
        }

        [Fact]
        public void EnvironmentVariableGetsPassed()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.Setup(x => x.Start(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                It.IsAny<int>()))
                .Returns(new ProcessResult() { ExitCode = -100 });
            string path = "/test";
            var target = new DotnetTestRunner(path, processMock.Object);

            target.SetActiveMutation(1);
            var result = target.RunAll();

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
