using System;
using System.Linq;
using DotNet.Globbing;
using Shouldly;
using Stryker.Core.ProjectComponents;
using Xunit;

namespace Stryker.Core.UnitTest.Options
{
    public class FilePatternTests : TestBase
    {
        [Theory]
        [InlineData("file.cs", "file.cs", true)]
        [InlineData("TestFolder/file.cs", "**/file.cs", true)]
        [InlineData("C:\\TestFolder\\file.cs", "**/file.cs", true)]
        [InlineData("C:\\TestFolder\\file.cs", "file.cs", false)]
        [InlineData("C:\\TestFolder\\file.cs", "./file.cs", false)]
        [InlineData("file.cs", "File.cs", false)]
        [InlineData("differentFile.cs", "file.cs", false)]
        [InlineData("File.cs", "*File.cs", true)]
        [InlineData("FileFile.cs", "*File.cs", true)]
        [InlineData("FileDifferent.cs", "*File.cs", false)]
        public void IsMatch_should_match_glob_pattern(string file, string glob, bool isMatch)
        {
            // Arrange
            var mutantSpan = new MutantSpan(0, 1);
            var pattern = new FilePattern(Glob.Parse(glob), false, new[] { mutantSpan });

            // Act
            var result = new CsharpFileLeaf().IsMatch(pattern, file, mutantSpan);

            // Assert
            result.ShouldBe(isMatch);
        }

        [Theory]
        [InlineData("{10..20}", 14, 16, true)]          // Fully contained
        [InlineData("{10..20}", 04, 06, false)]         // Before
        [InlineData("{10..20}", 24, 26, false)]         // After
        [InlineData("{10..20}", 14, 26, false)]         // Partial overlap; exceeds end
        [InlineData("{10..20}", 04, 16, false)]         // Partial overlap; exceeds start
        [InlineData("{10..20}{20..30}", 15, 25, true)]  // Two intersecting spans fully contain span
        [InlineData("{10..23}{17..30}", 15, 25, true)]  // Two overlapping spans fully contain span
        [InlineData("{10..19}{20..30}", 15, 25, false)] // Two spans with gab; start and end of span are in the spans
        public void IsMatch_should_match_textSpans(string spanPattern, int spanStart, int spanEnd, bool isMatch)
        {
            // Arrange
            var mutantSpan = new MutantSpan(spanStart, spanEnd);
            var pattern = new CsharpFileLeaf().Parse(new ExcludableString("*.*" + spanPattern));

            // Act
            var result = new CsharpFileLeaf().IsMatch(pattern, mutantSpan);

            // Assert
            result.ShouldBe(isMatch);
        }

        [Theory]
        [InlineData("**/*.cs{10..20}", "**/*.cs", false, new[] { 10, 20 })]
        [InlineData("**/*.cs{10..20}{20..30}", "**/*.cs", false, new[] { 10, 30 })]
        [InlineData("**/*.cs{10..20}{30..40}", "**/*.cs", false, new[] { 10, 20, 30, 40 })]
        [InlineData("!**/*.cs", "**/*.cs", true, new[] { 0, int.MaxValue })]
        public void Parse_should_parse_correctly(string spanPattern, string glob, bool isExclude, int[] spans)
        {
            // Arrange
            var mutantSpans = Enumerable.Range(0, spans.Length)
                .GroupBy(i => Math.Floor(i / 2d))
                .Select(x => new MutantSpan(spans[x.First()], spans[x.Skip(1).First()]));

            // Act
            var result = new CsharpFileLeaf().Parse(new ExcludableString(spanPattern));

            // Assert
            result.Glob.ToString().ShouldBe(FilePathUtils.NormalizePathSeparators(glob));
            result.IsExclude.ShouldBe(isExclude);
            result.MutantSpans.SequenceEqual(mutantSpans).ShouldBe(true);
        }
    }
}
