using System;
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
        [InlineData("Range", false)]
        [InlineData("*Range", false)]
        [InlineData("Ra*", false)]
        [InlineData("R*nge", false)]
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
        [InlineData("Range", false)]
        [InlineData("*Range", false)]
        [InlineData("Ra*", false)]
        [InlineData("R*nge", false)]
        [InlineData("", false)]
        public void MutantFilter_WorksWithConditionalInvocation(string ignoredMethodName, bool shouldSkipMutant)
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        var t = Enumerable.Range(0, 9)?.Select(x => x)?.Where(x => x < 5).ToList();
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
        [InlineData("Dispose")]
        [InlineData("Dispose*")]
        [InlineData("*Dispose")]
        [InlineData("*Dispose*")]
        [InlineData("*ispose")]
        [InlineData("Dis*")]
        [InlineData("*")]
        public void ShouldFilterStandaloneInvocation(string ignoredMethodName)
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        Dispose();
    }
}";
            var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
            var originalNode = baseSyntaxTree.FindNode(new TextSpan(source.IndexOf('D'), 1));

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
            filteredMutants.ShouldNotContain(mutant);
        }

        [Theory]
        [InlineData("Bar.Foo.Dispose", true)]
        [InlineData("Bar.*.Dispose", true)]
        [InlineData("Foo.Dispose*", true)]
        [InlineData("Foo.Dispos*", true)]
        [InlineData("*Foo.Dispose", true)]
        [InlineData("F*.Dispose", true)]
        [InlineData("*o.Dispose", true)]
        [InlineData("*o.D*se", true)]
        [InlineData("*.*", true)]
        [InlineData("Foo.*", true)]
        [InlineData("Foo*Dispose", false)]
        [InlineData("Bar.Foo", false)]
        [InlineData("Bar*", false)]
        [InlineData("Bar.", false)]
        [InlineData("Bar.*", false)]
        [InlineData("Foo", false)]
        [InlineData("Foo.", false)]
        [InlineData("*.*.*.*", false)]
        public void ShouldFilterInvocationWithQualifiedMemberName(string ignoredMethodName, bool shouldSkipMutant)
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        Bar // comment
            .Foo.Dispose();
    }
}";
            var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
            var originalNode = baseSyntaxTree.FindNode(new TextSpan(source.IndexOf('D'), 1));

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
        [InlineData("Foo.MyType.ctor", true)]
        [InlineData("MyType.ctor", true)]
        [InlineData("Foo.MyType*.ctor", true)]
        [InlineData("Foo*.MyType*.ctor", true)]
        [InlineData("*.MyType*.ctor", true)]
        [InlineData("F*.My*ype*.ctor", true)]
        [InlineData("MyType*.ctor", true)]
        [InlineData("*MyType.ctor", true)]
        [InlineData("*MyType*.ctor", true)]
        [InlineData("*Type.ctor", true)]
        [InlineData("My*.ctor", true)]
        [InlineData("*.ctor", true)]
        [InlineData("*.*.ctor", true)]
        [InlineData("MyType.constructor", false)]
        [InlineData("Type.ctor", false)]
        [InlineData("Foo.ctor", false)]
        public void MutantFilter_ShouldIgnoreConstructor(string ignoredMethodName, bool shouldSkipMutant)
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        var t = new Foo
                    .MyType(""Param"");
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
