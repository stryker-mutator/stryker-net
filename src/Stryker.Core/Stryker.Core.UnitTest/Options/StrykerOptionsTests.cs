using Serilog.Events;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Stryker.Core.UnitTest.Options
{
    public class StrykerOptionsTests
    {
        [Theory]
        [InlineData("error", LogEventLevel.Error)]
        [InlineData("", LogEventLevel.Information)]
        [InlineData(null, LogEventLevel.Information)]
        [InlineData("warning", LogEventLevel.Warning)]
        [InlineData("info", LogEventLevel.Information)]
        [InlineData("debug", LogEventLevel.Debug)]
        [InlineData("trace", LogEventLevel.Verbose)]
        public void Constructor_WithCorrectLoglevelArgument_ShouldAssignCorrectLogLevel(string argValue, LogEventLevel expectedLogLevel)
        {
            var options = new StrykerOptions(logLevel: argValue);

            options.LogOptions.ShouldNotBeNull();
            options.LogOptions.LogLevel.ShouldBe(expectedLogLevel);
        }

        [Fact]
        public void Constructor_WithIncorrectLoglevelArgument_ShouldThrowStrykerInputException()
        {
            var logLevel = "incorrect";

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerOptions(logLevel: logLevel);
            });

            ex.Message.ShouldBe("The value for one of your settings is not correct. Try correcting or removing them.");
        }

        [Fact]
        public void Constructor_WithIncorrectSettings_ShoudThrowWithDetails()
        {
            var logLevel = "incorrect";

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerOptions(logLevel: logLevel);
            });

            ex.Details.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void CustomTestProjectFilter_WithRelativePath_ShouldIncludeBasePath()
        {
            var userSuppliedFilter = "..\\ExampleActualTestProject\\TestProject.csproj";
            var basePath = FilePathUtils.ConvertPathSeparators(Path.Combine("C:", "ExampleProject", "TestProject"));
            var fullPath = FilePathUtils.ConvertPathSeparators(Path.Combine("C:", "ExampleProject", "ExampleActualTestProject", "TestProject.csproj"));

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerOptions(
                    basePath: basePath,
                    testProjectNameFilter: userSuppliedFilter);
            });

            ex.Details.ShouldContain($"The test project filter {userSuppliedFilter} is invalid.");
        }

        [Theory]
        [InlineData("./MyFolder/MyFile.cs", "MyFolder/MyFile.cs")]
        [InlineData("./MyFolder", "MyFolder/*.*")]
        [InlineData("C:/MyFolder/MyFile.cs", "C:/MyFolder/MyFile.cs")]
        public void FilesToExclude_should_be_converted_to_file_patterns(string fileToExclude, string expectedFilePattern)
        {
            // Act
            var result = new StrykerOptions(filesToExclude: new []{fileToExclude});

            // Assert
            var pattern = result.FilePatterns.Last();
            Path.GetFullPath(pattern.Glob.ToString()).ShouldBe(Path.GetFullPath(expectedFilePattern));
            pattern.TextSpans.ShouldContain(TextSpan.FromBounds(0, int.MaxValue));
            pattern.IsExclude.ShouldBeTrue();
        }
    }
}
