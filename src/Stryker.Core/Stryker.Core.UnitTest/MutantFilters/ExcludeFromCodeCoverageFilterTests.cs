using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Mutants;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;

namespace Stryker.Core.UnitTest.MutantFilters
{
    [TestClass]
    public class ExcludeFromCodeCoverageFilterTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveName()
        {
            var target = new ExcludeFromCodeCoverageFilter() as IMutantFilter;
            target.DisplayName.ShouldBe("exclude from code coverage filter");
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        [DataRow("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute")]
        [DataRow("ExcludeFromCodeCoverageAttribute")]
        [DataRow("ExcludeFromCodeCoverage")]
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
}
