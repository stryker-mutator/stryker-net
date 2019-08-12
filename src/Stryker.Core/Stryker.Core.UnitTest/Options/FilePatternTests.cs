using DotNet.Globbing;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Options
{
    public class FilePatternTests
    {
        [Theory]
        [InlineData("file.cs", "file.cs", true)]
        [InlineData("TestFolder/file.cs", "**/file.cs", true)]
        [InlineData("C:\\TestFolder\\file.cs", "**/file.cs", true)]
        [InlineData("file.cs", "File.cs", false)]
        [InlineData("differentFile.cs", "file.cs", false)]
        [InlineData("File.cs", "*File.cs", true)]
        [InlineData("FileFile.cs", "*File.cs", true)]
        [InlineData("FileDifferent.cs", "*File.cs", false)]
        public void IsMatch_should_match_glob_pattern(string file, string glob, bool isMatch)
        {
            // Arrange
            var textSpan = new TextSpan(0, 1);
            var sut = new FilePattern(Glob.Parse(glob), false, new[] {textSpan});

            // Act
            var result = sut.IsMatch(file, textSpan);

            // Assert
            result.ShouldBe(isMatch);
        }
    }
}