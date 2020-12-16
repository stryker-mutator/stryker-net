using System;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class IgnoredMethodMutantFilterTests
    {
        [Fact]
        public static void ShouldHaveName()
        {
            var target = new IgnoredMethodMutantFilter() as IMutantFilter;
            target.DisplayName.ShouldBe("method filter");
        }

        [Theory]
        [InlineData("Where", true)]
        [InlineData("Where*", true)]
        [InlineData("*Where", true)]
        [InlineData("*Where*", true)]
        [InlineData("*ere", true)]
        [InlineData("Wh*", true)]
        [InlineData("W*e", true)]
        [InlineData("*", true)]
        [InlineData("ToList", false)]
        [InlineData("*List", false)]
        [InlineData("To*", false)]
        [InlineData("T*ist", false)]
        [InlineData("", false)]
        public void MutantFilter_ChainedMethodsCalls(string ignoredMethodName, bool shouldSkipMutant)
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        var t = Enumerable.Range(0, 9).Where(x => x < 5).ToList();
    }
}";
            var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
            var originalNode = baseSyntaxTree.FindNode(new TextSpan(source.IndexOf('<'), 1));

            var mutant = new Mutant
            {
                Mutation = new Mutation
                {
                    OriginalNode = originalNode,
                }
            };

            var options = new StrykerOptions(ignoredMethods: new[] { ignoredMethodName });

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            if (shouldSkipMutant)
            {
                filteredMutants.ShouldNotContain(mutant);
            }
            else
            {
                filteredMutants.ShouldContain(mutant);
            }
        }

        [Theory]
        [InlineData("Where", true)]
        [InlineData("Where*", true)]
        [InlineData("*Where", true)]
        [InlineData("*Where*", true)]
        [InlineData("*ere", true)]
        [InlineData("Wh*", true)]
        [InlineData("W*e", true)]
        [InlineData("*", true)]
        [InlineData("ToList", false)]
        [InlineData("*List", false)]
        [InlineData("To*", false)]
        [InlineData("T*ist", false)]
        [InlineData("", false)]
        public void MutantFilter_WorksWithConditionalInvocation(string ignoredMethodName, bool shouldSkipMutant)
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        var t = Enumerable.Range(0, 9)?.Where(x => x < 5).ToList();
    }
}";
            var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
            var originalNode = baseSyntaxTree.FindNode(new TextSpan(source.IndexOf('<'), 1));

            var mutant = new Mutant
            {
                Mutation = new Mutation
                {
                    OriginalNode = originalNode,
                }
            };

            var options = new StrykerOptions(ignoredMethods: new[] { ignoredMethodName });

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            if (shouldSkipMutant)
            {
                filteredMutants.ShouldNotContain(mutant);
            }
            else
            {
                filteredMutants.ShouldContain(mutant);
            }
        }

        [Theory]
        [InlineData("MyType.ctor", true)]
        [InlineData("MyType*.ctor", true)]
        [InlineData("*MyType.ctor", true)]
        [InlineData("*MyType*.ctor", true)]
        [InlineData("*Type.ctor", true)]
        [InlineData("My*.ctor", true)]
        [InlineData("*.ctor", true)]
        [InlineData("MyType.constructor", false)]
        [InlineData("Type.ctor", false)]
        public void MutantFilter_ShouldIgnoreConstructor(string ignoredMethodName, bool shouldSkipMutant)
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        var t = new MyType(""Param"");
    }
}";
            var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
            var originalNode = baseSyntaxTree.FindNode(new TextSpan(source.IndexOf("Param", StringComparison.OrdinalIgnoreCase), 5));

            var mutant = new Mutant
            {
                Mutation = new Mutation
                {
                    OriginalNode = originalNode,
                }
            };

            var options = new StrykerOptions(ignoredMethods: new[] { ignoredMethodName });

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            if (shouldSkipMutant)
            {
                filteredMutants.ShouldNotContain(mutant);
            }
            else
            {
                filteredMutants.ShouldContain(mutant);
            }
        }

        [Fact]
        public void MutantFilters_ShouldNotApplyWithoutIgnoredMethod()
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        var t = Enumerable.Range(0, 9).Where(x => x < 5).ToList();
    }
}";
            var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
            var originalNode = baseSyntaxTree.FindNode(new TextSpan(source.IndexOf('<'), 1));

            var mutant = new Mutant
            {
                Mutation = new Mutation
                {
                    OriginalNode = originalNode,
                }
            };

            var options = new StrykerOptions();

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            filteredMutants.ShouldContain(mutant);
        }

        [Fact]
        public void MutantFilters_ConstructorFilterShouldNotMatchMethod()
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        myInstance.Myctor(
            $""This is my interpolatedString {myVariable}."");
    }
}";
            var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
            var originalNode = baseSyntaxTree.FindNode(new TextSpan(source.IndexOf("is", StringComparison.OrdinalIgnoreCase), 2));

            var mutant = new Mutant
            {
                Mutation = new Mutation
                {
                    OriginalNode = originalNode,
                }
            };

            var options = new StrykerOptions(ignoredMethods: new []{ "M.ctor" });

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            filteredMutants.ShouldContain(mutant);
        }

        [Fact]
        public void MutantFilters_ShouldIgnoreSyntaxWithoutInvocations()
        {
            // Arrange
            var originalNode = SyntaxFactory.BinaryExpression(
                SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var mutant = new Mutant
            {
                Mutation = new Mutation
                {
                    OriginalNode = originalNode,
                }
            };

            var options = new StrykerOptions(ignoredMethods: new[] { "Fact" });

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            filteredMutants.ShouldContain(mutant);
        }
    }
}
