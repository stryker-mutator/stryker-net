using System;
using System.Linq;
using DotNet.Globbing;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Utilities;

namespace Stryker.Core.UnitTest
{
    [TestClass]
    public class FilePatternTests : TestBase
    {
        [TestMethod]
        [DataRow("file.cs", "file.cs", true)]
        [DataRow("TestFolder/file.cs", "**/file.cs", true)]
        [DataRow("C:\\TestFolder\\file.cs", "**/file.cs", true)]
        [DataRow("C:\\TestFolder\\file.cs", "file.cs", false)]
        [DataRow("C:\\TestFolder\\file.cs", "./file.cs", false)]
        [DataRow("file.cs", "File.cs", false)]
        [DataRow("differentFile.cs", "file.cs", false)]
        [DataRow("File.cs", "*File.cs", true)]
        [DataRow("FileFile.cs", "*File.cs", true)]
        [DataRow("FileDifferent.cs", "*File.cs", false)]
        public void IsMatch_should_match_glob_pattern(string file, string glob, bool isMatch)
        {
            // Arrange
            var textSpan = new TextSpan(0, 1);
            var sut = new FilePattern(Glob.Parse(glob), false, new[] { textSpan });

            // Act
            var result = sut.IsMatch(file, textSpan);

            // Assert
            result.ShouldBe(isMatch);
        }

        [TestMethod]
        [DataRow("{10..20}", 14, 16, true)]          // Fully contained
        [DataRow("{10..20}", 04, 06, false)]         // Before
        [DataRow("{10..20}", 24, 26, false)]         // After
        [DataRow("{10..20}", 14, 26, false)]         // Partial overlap; exceeds end
        [DataRow("{10..20}", 04, 16, false)]         // Partial overlap; exceeds start
        [DataRow("{10..20}{20..30}", 15, 25, true)]  // Two intersecting spans fully contain span
        [DataRow("{10..23}{17..30}", 15, 25, true)]  // Two overlapping spans fully contain span
        [DataRow("{10..19}{20..30}", 15, 25, false)] // Two spans with gab; start and end of span are in the spans
        public void IsMatch_should_match_textSpans(string spanPattern, int spanStart, int spanEnd, bool isMatch)
        {
            // Arrange
            var sut = FilePattern.Parse("*.*" + spanPattern);

            // Act
            var result = sut.IsMatch($"test.cs", TextSpan.FromBounds(spanStart, spanEnd));

            // Assert
            result.ShouldBe(isMatch);
        }

        [TestMethod]
        [DataRow("**/*.cs{10..20}", "**/*.cs", false, new[] { 10, 20 })]
        [DataRow("**/*.cs{10..20}{20..30}", "**/*.cs", false, new[] { 10, 30 })]
        [DataRow("**/*.cs{10..20}{30..40}", "**/*.cs", false, new[] { 10, 20, 30, 40 })]
        [DataRow("!**/*.cs", "**/*.cs", true, new[] { 0, int.MaxValue })]
        public void Parse_should_parse_correctly(string spanPattern, string glob, bool isExclude, int[] spans)
        {
            // Arrange
            var textSpans = Enumerable.Range(0, spans.Length)
                .GroupBy(i => Math.Floor(i / 2d))
                .Select(x => TextSpan.FromBounds(spans[x.First()], spans[x.Skip(1).First()]));

            // Act
            var result = FilePattern.Parse(spanPattern);

            // Assert
            result.Glob.ToString().ShouldBe(FilePathUtils.NormalizePathSeparators(glob));
            result.IsExclude.ShouldBe(isExclude);
            result.TextSpans.SequenceEqual(textSpans).ShouldBe(true);
        }
    }
}
