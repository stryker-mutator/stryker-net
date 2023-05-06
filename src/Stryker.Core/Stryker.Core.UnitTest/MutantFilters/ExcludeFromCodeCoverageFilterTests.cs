using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters;

public class ExcludeFromCodeCoverageFilterTests : TestBase
{
    [Fact]
    public void ShouldHaveName()
    {
        var target = new ExcludeFromCodeCoverageFilter() as IMutantFilter;
        target.DisplayName.ShouldBe("exclude from code coverage filter");
    }

    [Fact]
    public void OnMethod()
    {
        // Arrange
        var mutant = Create(@"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    [ExcludeFromCodeCoverage]
    private void TestMethod()
    {
        var t = Enumerable.Range(0, 9).Where(x => x < 5).ToList();
    }
}", "<");

        var sut = new ExcludeFromCodeCoverageFilter() as IMutantFilter;

        // Act
        var results = sut.FilterMutants(new[] { mutant }, null, new StrykerOptions());

        // Assert
        results.ShouldNotContain(mutant);
    }

    [Fact]
    public void OnProperty()
    {
        // Arrange
        var mutant = Create(@"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    [ExcludeFromCodeCoverage]
    private string TestProperty => ""something""
}", "something");

        var sut = new ExcludeFromCodeCoverageFilter() as IMutantFilter;

        // Act
        var results = sut.FilterMutants(new[] { mutant }, null, new StrykerOptions());

        // Assert
        results.ShouldNotContain(mutant);
    }

    [Fact]
    public void OnClass()
    {
        // Arrange
        var mutant = Create(@"
[ExcludeFromCodeCoverage(""sowhat"")]
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        var t = Enumerable.Range(0, 9).Where(x => x < 5).ToList();
    }
}", "<");

        var sut = new ExcludeFromCodeCoverageFilter();

        // Act
        var results = sut.FilterMutants(new[] { mutant }, null, new StrykerOptions());

        // Assert
        results.ShouldNotContain(mutant);
    }

    [Fact]
    public void Not()
    {
        // Arrange
        var mutant = Create(@"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        var t = Enumerable.Range(0, 9).Where(x => x < 5).ToList();
    }
}", "<");

        var sut = new ExcludeFromCodeCoverageFilter();

        // Act
        var results = sut.FilterMutants(new[] { mutant }, null, new StrykerOptions());

        // Assert
        results.ShouldContain(mutant);
    }

    [Theory]
    [InlineData("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute")]
    [InlineData("ExcludeFromCodeCoverageAttribute")]
    [InlineData("ExcludeFromCodeCoverage")]
    public void Writings(string attr)
    {
        // Arrange
        var mutant = Create($@"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{{
    [{attr}]
    private void TestMethod()
    {{
        var t = Enumerable.Range(0, 9).Where(x => x < 5).ToList();
    }}
}}", "<");

        var sut = new ExcludeFromCodeCoverageFilter();

        // Act
        var results = sut.FilterMutants(new[] { mutant }, null, new StrykerOptions());

        // Assert
        results.ShouldNotContain(mutant);
    }


    private Mutant Create(string source, string search)
    {
        var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
        var originalNode =
            baseSyntaxTree.FindNode(new TextSpan(source.IndexOf(search, StringComparison.OrdinalIgnoreCase), 5));

        var mutant = new Mutant
        {
            Mutation = new Mutation
            {
                OriginalNode = originalNode,
            }
        };
        return mutant;
    }
}
