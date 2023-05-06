using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters;

public class IgnoredMethodMutantFilterTests : TestBase
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

        var options = new StrykerOptions
        {
            IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { ignoredMethodName } }.Validate()
        };

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
            IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { ignoredMethodName } }.Validate()
        };

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

        var options = new StrykerOptions
        {
            IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { ignoredMethodName } }.Validate()
        };

        var sut = new IgnoredMethodMutantFilter();

        // Act
        var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

        // Assert
        filteredMutants.ShouldNotContain(mutant);
    }

    [Fact]
    public void ShouldFilterStatementAndBlockWithOnlyIgnoredMethods()
    {
        // Arrange
        var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        Dispose();
        Dispose();
    }
}";
        var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
        var originalNode = (StatementSyntax)baseSyntaxTree.DescendantNodes().First(t => t is StatementSyntax and not BlockSyntax);

        var mutant = new Mutant
        {
            Mutation = new Mutation
            {
                OriginalNode = originalNode,
            }
        };

        var options = new StrykerOptions
        {
            IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { "Dispose" } }.Validate()
        };

        var blockMutant = new Mutant
        {
            Mutation = new Mutation
            {
                OriginalNode = (BlockSyntax)baseSyntaxTree.DescendantNodes().First(t => t is BlockSyntax),
            }
        };

        var sut = new IgnoredMethodMutantFilter();

        // Act
        var filteredMutants = sut.FilterMutants(new[] { mutant, blockMutant }, null, options);

        // Assert
        filteredMutants.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldFilterStatementAndBlockWithNonIgnoredMethods()
    {
        // Arrange
        var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        Dispose();
        for(;;)
            TestMethod();
        x = x+2;
   }
}";
        var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
        var originalNode = (StatementSyntax)baseSyntaxTree.DescendantNodes().First(t => t is StatementSyntax and not BlockSyntax);
        // this mutant is call to Dispose and should be ignored
        var mutant = new Mutant
        {
            Mutation = new Mutation
            {
                OriginalNode = originalNode,
            }
        };

        // this is an arbitrary statement mutation that should not be filtered
        originalNode = (StatementSyntax)baseSyntaxTree.DescendantNodes().Where(t => t is StatementSyntax and not BlockSyntax).ElementAt(3);

        var statementMutant = new Mutant
        {
            Mutation = new Mutation
            {
                OriginalNode = originalNode,
            }
        };

        // this is a block mutations that contains non ignored methods it should not be filtered
        var blockMutant = new Mutant
        {
            Mutation = new Mutation
            {
                OriginalNode = (BlockSyntax)baseSyntaxTree.DescendantNodes().First(t => t is BlockSyntax),
            }
        };

        var options = new StrykerOptions
        {
            IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { "Dispose" } }.Validate()
        };

        var sut = new IgnoredMethodMutantFilter();

        // Act
        var filteredMutants = sut.FilterMutants(new[] { mutant, blockMutant, statementMutant }, null, options);

        // Assert
        filteredMutants.ShouldNotContain(mutant);
        filteredMutants.ShouldContain(blockMutant);
        filteredMutants.ShouldContain(statementMutant);
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
        var originalNode = baseSyntaxTree.FindNode(new TextSpan(source.IndexOf("Dispose"), 1));

        var mutant = new Mutant
        {
            Mutation = new Mutation
            {
                OriginalNode = originalNode,
            }
        };

        var options = new StrykerOptions
        {
            IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { ignoredMethodName } }.Validate()
        };

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

        var options = new StrykerOptions
        {
            IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { ignoredMethodName } }.Validate()
        };

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

        var options = new StrykerOptions
        {
            IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { "M.ctor" } }.Validate()
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
            IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { "Fact" } }.Validate()
        };

        var sut = new IgnoredMethodMutantFilter();

        // Act
        var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

        // Assert
        filteredMutants.ShouldContain(mutant);
    }

    [Fact]
    public void MutantFilters_DoesNotIgnoreOtherMutantsInFile()
    {
        // Arrange
        var source = @"
public class MutantFilters_DoNotIgnoreOtherMutantsInFile
{
    private void TestMethod()
    {
        Foo(true);
        Bar(""A Mutation"");
        Quux(42);
    }
}";
        var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
        var mutants = new[] { "true", @"""A Mutation""", "42"}.Select(GetOriginalNode).Select(node => new Mutant { Mutation = new Mutation { OriginalNode = node } }).ToArray();
        var options = new StrykerOptions
        {
            IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { "Bar" } }.Validate()
        };
        var sut = new IgnoredMethodMutantFilter();

        // Act
        var filteredMutants = sut.FilterMutants(mutants, null, options);

        // Assert
        filteredMutants.ShouldContain(mutants[0]); // Foo(true);
        filteredMutants.ShouldNotContain(mutants[1]); // Bar(""A Mutation"");
        filteredMutants.ShouldContain(mutants[2]); // Quux(42);

        Microsoft.CodeAnalysis.SyntaxNode GetOriginalNode(string node) =>
            baseSyntaxTree.FindNode(new TextSpan(source.IndexOf(node, StringComparison.OrdinalIgnoreCase), node.Length));
    }

    [Fact]
    public void MutantFilters_DoesNotIgnoreMethodDeclaration()
    {
        // Arrange
        var source = @"
public class MutantFilters_DoNotIgnoreOtherMutantsInFile
{
    private void TestMethod()
    {
        Foo(true);
        Bar(""A Mutation"");
        Quux(42);
    }
}";
        var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
        var mutants = new[] { "true", @"""A Mutation""", "42" }.Select(GetOriginalNode).Select(node => new Mutant { Mutation = new Mutation { OriginalNode = node } }).ToArray();
        var options = new StrykerOptions
        {
            IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { "TestMethod" } }.Validate()
        };
        var sut = new IgnoredMethodMutantFilter();

        // Act
        var filteredMutants = sut.FilterMutants(mutants, null, options);

        // Assert
        filteredMutants.ShouldContain(mutants[0]); // Foo(true);
        filteredMutants.ShouldContain(mutants[1]); // Bar(""A Mutation"");
        filteredMutants.ShouldContain(mutants[2]); // Quux(42);

        Microsoft.CodeAnalysis.SyntaxNode GetOriginalNode(string node) =>
            baseSyntaxTree.FindNode(new TextSpan(source.IndexOf(node, StringComparison.OrdinalIgnoreCase), node.Length));
    }
}
