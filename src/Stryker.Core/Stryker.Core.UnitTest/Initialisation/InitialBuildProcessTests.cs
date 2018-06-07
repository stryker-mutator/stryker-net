using Moq;
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
        public void InitialBuildProcess_ShouldThrowExceptionOnFail()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.Setup(x => x.Start(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(new ProcessResult() { ExitCode = 1 });

            var target = new InitialBuildProcess(processMock.Object);

            var exception = Assert.Throws<Exception>(() => target.InitialBuild("/", "ExampleProject.csproj"));
        }

        [Fact]
        public void InitialBuildProcess_ShouldNotThrowExceptionOnSuccess()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            processMock.Setup(x => x.Start(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(new ProcessResult() { ExitCode = 0 });

            var target = new InitialBuildProcess(processMock.Object);

            target.InitialBuild("/", "ExampleProject.csproj");
        }
    }
}
