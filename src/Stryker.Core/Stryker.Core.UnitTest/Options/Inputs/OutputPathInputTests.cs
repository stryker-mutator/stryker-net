using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class OutputPathInputTests
    {
        [Theory]
        [InlineData("StrykerOutput")]
        [InlineData("test")]
        public void ShouldReturnValidOutputPath(string outputPath)
        {
            var target = new OutputPathInput { SuppliedInput = outputPath };
            var fileSystemMock = new MockFileSystem();
            var loggerMock = new Mock<ILogger>();

            var result = target.Validate(loggerMock.Object, fileSystemMock, Path.Combine("C:" , "Test"));

            result.ShouldStartWith(Path.Combine("C:", "Test", outputPath, DateTime.Now.ToString("yyyy-MM-dd.HH-")));

            new Regex(@"\d{4}-\d{2}-\d{2}.\d{2}-\d{2}-\d{2}").Match(result).Success.ShouldBeTrue();
        }
    }
}
