using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class IgnoredMethodMutantFilterTests : TestBase
    {


        [Fact]
        public static void ShouldHaveName()
        {
            var target = new IgnoredMethodMutantFilter() as IMutantFilter;
            target.DisplayName.ShouldBe("method filter");
        }

        // filter should support documented cases
        [Theory]
        [InlineData("IgnoredMethod(true);", "true", true)]
        [InlineData("x = IgnoredMethod(true);", null, true)]
        [InlineData("var x = IgnoredMethod(true);", null, true)]
        [InlineData("while (x == IgnoredMethod(true));", "==", false)]
        [InlineData("IgnoredMethod()++;", "IgnoredMethod()++", false)]
        [InlineData("x==1 ? IgnoredMethod(true): IgnoredMethod(false);", "==", true)]
        [InlineData("SomeMethod(true).IgnoredMethod(false);", "true", true)]
        public void ShouldFilterDocumentedCases(string methodCall, string anchor, bool shouldSkipMutant)
        {
            // Arrange
            var source = $@"public void StubMethod()
{{
    {methodCall}
}}";
            anchor ??= methodCall;
            var options = new StrykerOptions
            {
                IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { "IgnoredMethod" } }.Validate()
            };

            var sut = new IgnoredMethodMutantFilter();
            foreach(var (mutant, label) in BuildMutantsToFilter(source, anchor))
            {
                // Act
                var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

                // Assert
                if (shouldSkipMutant)
                {
                    filteredMutants.ShouldNotContain(mutant, $"{label} should have been filtered out.");
                }
                else
                {
                    filteredMutants.ShouldContain(mutant, $"{label} should have been kept.");
                }
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
        [InlineData("ToList", true)]
        [InlineData("*List", true)]
        [InlineData("To*", true)]
        [InlineData("T*ist", true)]
        [InlineData("Range", false)]
        [InlineData("*Range", false)]
        [InlineData("Ra*", false)]
        [InlineData("R*nge", false)]
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

            var options = new StrykerOptions
            {
                IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { ignoredMethodName } }.Validate()
            };

            var sut = new IgnoredMethodMutantFilter();

           var (mutant, label) = BuildExpressionMutant(source, "<");
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            if (shouldSkipMutant)
            {
                filteredMutants.ShouldNotContain(mutant, $"{label} should have been filtered out.");
            }
            else
            {
                filteredMutants.ShouldContain(mutant, $"{label} should have been kept.");
            }
        }

        [Theory]
        [InlineData("Range", false)]
        [InlineData("Where", false)]
        [InlineData("ToList", true)]
        [InlineData("", false)]
        public void MutantFilter_ChainedMethodsCallStatement(string ignoredMethodName, bool shouldSkipMutant)
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        Enumerable.Range(0, 9).Where(x => x < 5).ToList();
    }
}";

            var options = new StrykerOptions
            {
                IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { ignoredMethodName } }.Validate()
            };

            var sut = new IgnoredMethodMutantFilter();
            foreach(var (mutant, label) in BuildMutantsToFilter(source, "ToList"))
            {
                // Act
                var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

                // Assert
                if (shouldSkipMutant)
                {
                    filteredMutants.ShouldNotContain(mutant, $"{label} should have been filtered out.");
                }
                else
                {
                    filteredMutants.ShouldContain(mutant, $"{label} should have been kept.");
                }
            }
        }

        [Theory]
        [InlineData("Where", true)]
        [InlineData("ToList", true)]
        [InlineData("Range", false)]
        public void MutantFilter_WorksWithConditionalInvocation(string ignoredMethodName, bool shouldSkipMutant)
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        Enumerable.Range(0, 9)?.Where(x => x < 5).ToList();
    }
}";

            var options = new StrykerOptions
            {
                IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { ignoredMethodName } }.Validate()
            };

            var sut = new IgnoredMethodMutantFilter();

            var mutant = BuildExpressionMutant(source, "<").Item1;

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
        [InlineData("Range", false)]
        [InlineData("Where", false)]
        [InlineData("ToList", true)]
        public void MutantFilter_WorksWithConditionalInvocationStatement(string ignoredMethodName, bool shouldSkipMutant)
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        Enumerable.Range(0, 9)?.Where(x => x < 5)?.ToList();
    }
}";
            var options = new StrykerOptions
            {
                IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { ignoredMethodName } }.Validate()
            };

            var sut = new IgnoredMethodMutantFilter();

            foreach( var (mutant, label) in BuildMutantsToFilter(source, "ToList"))
            {
                // Act
                var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

                // Assert
                if (shouldSkipMutant)
                {
                    filteredMutants.ShouldNotContain(mutant, $"{label} should have been filtered out.");
                }
                else
                {
                    filteredMutants.ShouldContain(mutant, $"{label} should have been kept.");
                }
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
            var mutant = BuildExpressionMutant(source, "Dispose").Item1;

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
            var originalNode = FindEnclosingNode<StatementSyntax>(baseSyntaxTree, "Dispose");

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
            var options = new StrykerOptions
            {
                IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { "Dispose" } }.Validate()
            };
            var sut = new IgnoredMethodMutantFilter();

            foreach(var (mutant, label) in BuildMutantsToFilter(source, "Dispose"))
            {
                // act
                var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);
                // Assert
                if (mutant.Mutation.OriginalNode is BlockSyntax)
                {
                    filteredMutants.ShouldContain(mutant, $"{label} should have been kept.");
                }
                else
                {
                    filteredMutants.ShouldBeEmpty();
                }
            }
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
            var originalNode = FindEnclosingNode<StatementSyntax>(baseSyntaxTree, "Dispose");

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
            var originalNode = FindEnclosingNode<SyntaxNode>(baseSyntaxTree, "Param");

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
            var originalNode = FindEnclosingNode<SyntaxNode>(baseSyntaxTree, "<");

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
            var originalNode = FindEnclosingNode<SyntaxNode>(baseSyntaxTree, "is");

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

        private T FindEnclosingNode<T>(SyntaxNode start) where T: SyntaxNode =>
            start switch
            {
                null => null,
                T t => t,
                _ => FindEnclosingNode<T>(start.Parent)
            };

        private T FindEnclosingNode<T>(SyntaxNode start, string anchor) where T: SyntaxNode => FindEnclosingNode<T>(start.FindNode(new TextSpan(start.ToFullString().IndexOf(anchor), anchor.Length)));

        private IEnumerable<(Mutant, string)> BuildMutantsToFilter(string csharp, string anchor)
        {
            var baseSyntaxTree = CSharpSyntaxTree.ParseText(csharp).GetRoot();

            var originalNode = FindEnclosingNode<ExpressionSyntax>(baseSyntaxTree, anchor);
            if (originalNode != null)
            {
                yield return (new Mutant
                {
                    Mutation = new Mutation
                    {
                        OriginalNode = originalNode,
                    }
                }, "Expression mutant");
            }

            yield return (new Mutant
            {
                Mutation = new Mutation
                {
                    OriginalNode = FindEnclosingNode<StatementSyntax>(baseSyntaxTree, anchor),
                }
            }, "Statement mutant");

            yield return (new Mutant
            {
                Mutation = new Mutation
                {
                    OriginalNode = FindEnclosingNode<BlockSyntax>(baseSyntaxTree, anchor),
                }
            }, "Block mutant");

        }

        private (Mutant, string) BuildExpressionMutant(string sourceCode, string anchor)
        {
            var mutant = new Mutant
            {
                Mutation = new Mutation
                {
                    OriginalNode = FindEnclosingNode<ExpressionSyntax>(CSharpSyntaxTree.ParseText(sourceCode).GetRoot(), anchor)
                }
            };
            return (mutant, "Expression");
        }
    }
}
