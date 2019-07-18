using Moq;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;
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

            string path = FilePathUtils.ConvertPathSeparators("c://test");
            var target = new DotnetTestRunner(path, processMock.Object, OptimizationFlags.NoOptimization);

            var result = target.RunAll(null, null);

            Assert.True(result.Success);
            processMock.Verify(m => m.Start(path, "dotnet", It.Is<string>(s => s.Contains("test")), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<int>()));
        }

        [Fact]
        public void RunAllExitCode1SuccessShouldBeFalse()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.SetupProcessMockToReturn("Testrun failed", 1);

            string path = FilePathUtils.ConvertPathSeparators("c://test");
            var target = new DotnetTestRunner(path, processMock.Object, OptimizationFlags.NoOptimization);

            var result = target.RunAll(null, null);

            Assert.False(result.Success);
            processMock.Verify(m => m.Start(path, "dotnet", It.Is<string>(s => s.Contains("test")), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<int>()));
        }

        [Fact]
        public void RunAllExceptionStatusCodeSuccessShouldBeFalse()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.SetupProcessMockToReturn("Testrun failed other way", -100);

            string path = FilePathUtils.ConvertPathSeparators("c://test");
            var target = new DotnetTestRunner(path, processMock.Object, OptimizationFlags.NoOptimization);

            var result = target.RunAll(null, null);

            Assert.False(result.Success);
            processMock.Verify(m => m.Start(path, "dotnet", It.Is<string>(s => s.Contains("test")), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<int>()));
        }

        [Fact]
        public void EnvironmentVariableGetsPassed()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.SetupProcessMockToReturn("Testrun failed other way", -100);

            string path = FilePathUtils.ConvertPathSeparators("c://test");
            var target = new DotnetTestRunner(path, processMock.Object, OptimizationFlags.NoOptimization);

            var result = target.RunAll(null, new Mutant(){Id =  1});

            Assert.False(result.Success);
            processMock.Verify(m => m.Start(
                path,
                "dotnet",
                It.Is<string>(s => s.Contains("test")),
                It.Is<IDictionary<string, string>>(x => x.Any(y => y.Value == "1" && y.Key == "ActiveMutation")),
                It.IsAny<int>()));
        }
    }
}
