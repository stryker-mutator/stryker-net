using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public static class ExcludeFromCodeCoverageFilterTests
    {
        [Fact]
        public static void ShouldHaveName()
        {
            var target = new ExcludeFromCodeCoverageFilter() as IMutantFilter;
            target.DisplayName.ShouldBe("exclude from code coverage filter");
        }

        [Fact]
        public static void OnMethod()
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
        public static void OnProperty()
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
        public static void OnClass()
        {
            // Arrange
            var mutant = Create(@"
[ExcludeFromCodeCoverage]
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
        public static void Not()
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
        public static void Writings(string attr)
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


        private static Mutant Create(string source, string search)
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
}
