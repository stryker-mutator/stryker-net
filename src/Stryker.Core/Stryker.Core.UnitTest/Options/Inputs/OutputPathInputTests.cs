using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class OutputPathInputTests
    {
        [Fact]
        public void ShouldReturnValidOutputPath()
        {
            var target = new OutputPathInput { SuppliedInput = "StrykerOutput" };
            var fileSystemMock = new MockFileSystem();
            var loggerMock = new Mock<ILogger>();

            var result = target.Validate(loggerMock.Object, fileSystemMock, "C:/Test");

            result.ShouldBe("C:/Test/StrykerOutput");
        }
    }
}
