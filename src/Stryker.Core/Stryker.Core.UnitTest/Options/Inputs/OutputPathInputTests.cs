using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
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
        private MockFileSystem _fileSystemMock = new MockFileSystem();
        private Mock<ILogger> _loggerMock = new Mock<ILogger>();

        [Fact]
        public void ShouldHaveHelptext()
        {
            var target = new OutputPathInput();
            target.HelpText.ShouldBe(@"");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldHaveDefault(string outputPath)
        {
            var target = new OutputPathInput { SuppliedInput = outputPath };

            var result = target.Validate(_loggerMock.Object, _fileSystemMock, Path.Combine("C:", "Test"));

            result.ShouldStartWith(Path.Combine("C:", "Test", "StrykerOutput", DateTime.Now.ToString("yyyy-MM-dd.HH-")));
        }

        [Theory]
        [InlineData("StrykerOutput")]
        [InlineData("test")]
        public void ShouldReturnValidOutputPath(string outputPath)
        {
            var target = new OutputPathInput { SuppliedInput = outputPath };

            var result = target.Validate(_loggerMock.Object, _fileSystemMock, Path.Combine("C:" , "Test"));

            // we can't assert the complete datetime as it will differ in milliseconds
            result.ShouldStartWith(Path.Combine("C:", "Test", outputPath, DateTime.Now.ToString("yyyy-MM-dd.HH-")));

            // assert the format of the dir name is correct
            var dateTimeRegex = new Regex(@"\d{4}-\d{2}-\d{2}.\d{2}-\d{2}-\d{2}");
            dateTimeRegex.Match(result).Success.ShouldBeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldThrowOnEmptyValue(string value)
        {
            var input = new OutputPathInput { SuppliedInput = value };

            var exception = Should.Throw<ArgumentNullException>(() =>
            {
                input.Validate(_loggerMock.Object, _fileSystemMock, value);
            });

            exception.Message.ShouldBe("Value cannot be null. (Parameter 'basepath')");
        }

        [Theory]
        [InlineData("StrykerOutput")]
        [InlineData("test")]
        public void ShouldAddGitIgnore(string outputPath)
        {
            var target = new OutputPathInput { SuppliedInput = outputPath };

            var result = target.Validate(_loggerMock.Object, _fileSystemMock, Path.Combine("C:", "Test"));

            var gitIgnoreFile = _fileSystemMock.AllFiles.FirstOrDefault(x => x.EndsWith(Path.Combine(outputPath, ".gitignore")));
            gitIgnoreFile.ShouldNotBeNull();
            var fileContents = _fileSystemMock.GetFile(gitIgnoreFile).Contents;
            Encoding.Default.GetString(fileContents).ShouldBe("*");
        }
    }
}
