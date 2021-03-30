using Moq;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
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

            var path = FilePathUtils.NormalizePathSeparators("c://test");
            var target = new DotnetTestRunner(path, processMock.Object, OptimizationFlags.NoOptimization, new[] { "C://test//mytest.dll" });

            var result = target.RunAll(null, null, null);

            result.FailingTests.Count.ShouldBe(0);
            processMock.Verify(m => m.Start(path, "dotnet", It.Is<string>(s => s.Contains("vstest C://test//mytest.dll")), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<int>()));
        }

        [Fact]
        public void RunAllExitCode1SuccessShouldBeFalse()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.SetupProcessMockToReturn("Testrun failed", 1);

            var path = FilePathUtils.NormalizePathSeparators("c://test");
            var target = new DotnetTestRunner(path, processMock.Object, OptimizationFlags.NoOptimization, new[] { "C://test//mytest.dll" });

            var result = target.RunAll(null, null, null);

            Assert.True(result.FailingTests.IsEveryTest);
            processMock.Verify(m => m.Start(path, "dotnet", It.Is<string>(s => s.Contains("vstest")), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<int>()));
        }

        [Fact]
        public void RunAllExceptionStatusCodeSuccessShouldBeFalse()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.SetupProcessMockToReturn("Testrun failed other way", -100);

            string path = FilePathUtils.NormalizePathSeparators("c://test");
            var target = new DotnetTestRunner(path, processMock.Object, OptimizationFlags.NoOptimization, new[] { "C://test//mytest.dll" });

            var result = target.RunAll(null, null, null);

            Assert.True(result.FailingTests.IsEveryTest);
            processMock.Verify(m => m.Start(path, "dotnet", It.Is<string>(s => s.Contains("vstest")), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<int>()));
        }

        [Fact]
        public void EnvironmentVariableGetsPassed()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.SetupProcessMockToReturn("Testrun failed other way");

            string path = FilePathUtils.NormalizePathSeparators("c://test");
            var target = new DotnetTestRunner(path, processMock.Object, OptimizationFlags.NoOptimization, new[] { "C://test//mytest.dll" });

            var result = target.RunAll(null, new Mutant(){Id =  1}, null);

            Assert.Equal(0, result.FailingTests.Count);
            processMock.Verify(m => m.Start(
                path,
                "dotnet",
                It.Is<string>(s => s.Contains("vstest")),
                It.Is<IDictionary<string, string>>(x => x.Any(y => y.Value == "1" && y.Key == "ActiveMutation")),
                It.IsAny<int>()));
        }
    }
}
