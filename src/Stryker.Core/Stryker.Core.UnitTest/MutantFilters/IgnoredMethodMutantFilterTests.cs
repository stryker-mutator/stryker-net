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
        [InlineData("^Where$")]
        [InlineData("^Where*$")]
        [InlineData("^*Where$")]
        [InlineData("^*Where$*")]
        [InlineData("^*ere$")]
        [InlineData("^Wh.*$")]
        [InlineData("^W.*e$")]
        [InlineData("^*$")]
        public void ShouldIgnoreMethod(string ignoredMethodName)
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

            var options = new StrykerOptions
            {
                IgnoredMethods = new[] { new Regex(ignoredMethodName) }
            };

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            filteredMutants.ShouldNotContain(mutant);
        }

        [Theory]
        [InlineData("^ToList$")]
        [InlineData("^*List$")]
        [InlineData("^To*$")]
        [InlineData("^T*ist$")]
        [InlineData("^$")]
        public void ShouldNotIgnoreMethod(string ignoredMethodName)
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

            var options = new StrykerOptions
            {
                IgnoredMethods = new[] { new Regex(ignoredMethodName) }
            };

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            filteredMutants.ShouldContain(mutant);
        }

        [Theory]
        [InlineData("^Where$")]
        [InlineData("^Where.*$")]
        [InlineData("^*Where$")]
        [InlineData("^*Where.*$")]
        [InlineData("^.*ere$")]
        [InlineData("^Wh.*$")]
        [InlineData("^W.*e$")]
        [InlineData("^.*$")]
        public void ShouldIgnoreWithConditionalInvocation(string ignoredMethodName)
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

            var options = new StrykerOptions
            {
                IgnoredMethods = new[] { new Regex(ignoredMethodName) }
            };

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            filteredMutants.ShouldNotContain(mutant);
        }

        [Theory]
        [InlineData("^ToList$")]
        [InlineData("^*List$")]
        [InlineData("^To.*$")]
        [InlineData("^T*ist$")]
        [InlineData("^$")]
        public void ShouldNotIgnoreWithConditionalInvocation(string ignoredMethodName)
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

            var options = new StrykerOptions
            {
                IgnoredMethods = new[] { new Regex(ignoredMethodName) }
            };

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            filteredMutants.ShouldContain(mutant);
        }

        [Theory]
        [InlineData("^MyType.ctor$")]
        [InlineData("^MyType.*.ctor$")]
        [InlineData("^*MyType.ctor$")]
        [InlineData("^*MyType.*.ctor$")]
        [InlineData("^*Type.ctor$")]
        [InlineData("^My.*.ctor$")]
        [InlineData("^*.ctor$")]
        public void ShouldIgnoreConstructor(string ignoredMethodName)
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

            var options = new StrykerOptions
            {
                IgnoredMethods = new[] { new Regex(ignoredMethodName) }
            };

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            filteredMutants.ShouldNotContain(mutant);
        }

        [Theory]
        [InlineData("^MyType.constructor$")]
        [InlineData("^Type.ctor$")]
        public void ShouldNotIgnoreConstructor(string ignoredMethodName)
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

            var options = new StrykerOptions
            {
                IgnoredMethods = new[] { new Regex(ignoredMethodName) }
            };

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            filteredMutants.ShouldContain(mutant);
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

            var options = new StrykerOptions
            {
                IgnoredMethods = new[] { new Regex("^M.ctor^") }
            };

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

            var options = new StrykerOptions
            {
                IgnoredMethods = new[] { new Regex("Fact") }
            };

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            filteredMutants.ShouldContain(mutant);
        }
    }
}
