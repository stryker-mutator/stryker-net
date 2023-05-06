using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters;

public class FilePatternMutantFilterTests : TestBase
{
    [Fact]
    public static void ShouldHaveName()
    {
        var target = new FilePatternMutantFilter(new StrykerOptions()) as IMutantFilter;
        target.DisplayName.ShouldBe("mutate filter");
    }

    [Theory]
    [InlineData(new[] { "**/*" }, "myFolder/MyFile.cs", 5, 10, true)]
    [InlineData(new[] { "**/*File.cs" }, "myFolder/MyFile.cs", 5, 10, true)]
    [InlineData(new[] { "**/*File.cs" }, "myFolder/MyFileSomething.cs", 5, 10, false)]
    [InlineData(new[] { "**/*", "!**/MyFile.cs" }, "myFolder/MyFile.cs", 5, 10, false)]
    [InlineData(new[] { "**/*", "!**/MyFile.cs", "**/*" }, "myFolder/MyFile.cs", 5, 10, false)]
    [InlineData(new[] { "**/*", "!MyFile.cs" }, "myFolder/MyFile.cs", 5, 10, true)]
    [InlineData(new[] { "**/*", "!**/MyFile.cs{3..13}" }, "myFolder/MyFile.cs", 5, 10, false)]
    [InlineData(new[] { "**/*", "!**/MyFile.cs{1..7}{7..13}" }, "myFolder/MyFile.cs", 5, 10, false)]
    [InlineData(new[] { "**/*", "!**/MyFile.cs{1..3}{5..10}" }, "myFolder/MyFile.cs", 5, 10, false)]
    [InlineData(new[] { "**/*", "!**/MyFile.cs{1..7}" }, "myFolder/MyFile.cs", 5, 10, true)]
    [InlineData(new[] { "**/*", "!**/MyFile.cs{7..13}" }, "myFolder/MyFile.cs", 5, 10, true)]
    [InlineData(new[] { "**/*", "!C:/test/myFolder/MyFile.cs" }, "myFolder/MyFile.cs", 5, 10, false)]
    [InlineData(new[] { "**/*", "!C:\\test\\myFolder\\MyFile.cs" }, "myFolder/MyFile.cs", 5, 10, false)]
    public void FilterMutants_should_filter_included_and_excluded_files(
        string[] patterns,
        string filePath,
        int spanStart,
        int spanEnd,
        bool shouldKeepFile)
    {
        // Arrange
        var options = new StrykerOptions() { Mutate = patterns.Select(FilePattern.Parse) };
        var file = new CsharpFileLeaf { RelativePath = filePath, FullPath = Path.Combine("C:/test/", filePath) };

        // Create token with the correct text span
        var syntaxToken = SyntaxFactory.Identifier(
            new SyntaxTriviaList(Enumerable.Range(0, spanStart).Select(x => SyntaxFactory.Space)),
            new string('a', spanEnd - spanStart),
            SyntaxTriviaList.Empty);

        var mutant = new Mutant
        { Mutation = new Mutation { OriginalNode = SyntaxFactory.IdentifierName(syntaxToken) } };

        var sut = new FilePatternMutantFilter(options);

        // Act
        var result = sut.FilterMutants(new[] { mutant }, file, null);

        // Assert
        result.Contains(mutant).ShouldBe(shouldKeepFile);
    }
}
