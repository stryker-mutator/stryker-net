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

            processMock.SetupProcessMockToReturn("", 1);

            var target = new InitialBuildProcess(processMock.Object);

            var exception = Assert.Throws<Exception>(() => target.InitialBuild("/", "ExampleProject.csproj"));
        }

        [Fact]
        public void InitialBuildProcess_ShouldNotThrowExceptionOnSuccess()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

            processMock.SetupProcessMockToReturn("");

            var target = new InitialBuildProcess(processMock.Object);

            target.InitialBuild("/", "ExampleProject.csproj");
        }
    }
}
