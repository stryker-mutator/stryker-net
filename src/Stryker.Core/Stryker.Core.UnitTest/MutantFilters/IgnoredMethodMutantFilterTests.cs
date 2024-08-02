using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Configuration.MutantFilters;
using Stryker.Configuration.Mutants;
using Stryker.Configuration;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.MutantFilters
{
    [TestClass]
    public class IgnoredMethodMutantFilterTests : TestBase
    {


        [TestMethod]
        public static void ShouldHaveName()
        {
            var target = new IgnoredMethodMutantFilter() as IMutantFilter;
            target.DisplayName.ShouldBe("method filter");
        }

        // filter should support documented cases
        [TestMethod]
        [DataRow("IgnoredMethod(true);", "true", true)]
        [DataRow("x = IgnoredMethod(true);", null, true)]
        [DataRow("var x = IgnoredMethod(true);", null, true)]
        [DataRow("while (x == IgnoredMethod(true));", "==", false)]
        [DataRow("IgnoredMethod()++;", "IgnoredMethod()++", false)]
        [DataRow("x==1 ? IgnoredMethod(true): IgnoredMethod(false);", "==", true)]
        [DataRow("SomeMethod(true).IgnoredMethod(false);", "true", true)]
        [DataRow("IgnoredMethod(x==> SomeCall(param));", "param", true)]
        [DataRow("IgnoredMethod(x==1 ? true : false);", "false", true)]
        public void ShouldFilterDocumentedCases(string methodCall, string anchor, bool shouldSkipMutant)
        {
            // Arrange
            var source = $@"class Test{{public void StubMethod() =>

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

        [TestMethod]
        [DataRow("Where", true)]
        [DataRow("Where*", true)]
        [DataRow("*Where", true)]
        [DataRow("*Where*", true)]
        [DataRow("*ere", true)]
        [DataRow("Wh*", true)]
        [DataRow("W*e", true)]
        [DataRow("*", true)]
        [DataRow("ToList", true)]
        [DataRow("*List", true)]
        [DataRow("To*", true)]
        [DataRow("T*ist", true)]
        [DataRow("Range", false)]
        [DataRow("*Range", false)]
        [DataRow("Ra*", false)]
        [DataRow("R*nge", false)]
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

        [TestMethod]
        [DataRow("Range", false)]
        [DataRow("Where", false)]
        [DataRow("ToList", true)]
        [DataRow("", false)]
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

        [TestMethod]
        [DataRow("Where", true)]
        [DataRow("ToList", true)]
        [DataRow("Range", false)]
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

        [TestMethod]
        [DataRow("Range", false)]
        [DataRow("Where", false)]
        [DataRow("ToList", true)]
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

        [TestMethod]
        public void MutantFilter_WorksWithGenericMethodCalls()
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        Enumerable.Range(0, 9)?.Where(x => x < 5)?.ToList<int>();
    }
}";
            var options = new StrykerOptions
            {
                IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = new[] { "ToList" } }.Validate()
            };

            var sut = new IgnoredMethodMutantFilter();

            foreach( var (mutant, label) in BuildMutantsToFilter(source, "ToList"))
            {
                // Act
                var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

                // Assert
                filteredMutants.ShouldNotContain(mutant, $"{label} should have been filtered out.");
            }
        }

        [TestMethod]
        [DataRow("Dispose")]
        [DataRow("Dispose*")]
        [DataRow("*Dispose")]
        [DataRow("*Dispose*")]
        [DataRow("*ispose")]
        [DataRow("Dis*")]
        [DataRow("*")]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        [DataRow("Bar.Foo.Dispose", true)]
        [DataRow("Bar.*.Dispose", true)]
        [DataRow("Foo.Dispose*", true)]
        [DataRow("Foo.Dispos*", true)]
        [DataRow("*Foo.Dispose", true)]
        [DataRow("F*.Dispose", true)]
        [DataRow("*o.Dispose", true)]
        [DataRow("*o.D*se", true)]
        [DataRow("*.*", true)]
        [DataRow("Foo.*", true)]
        [DataRow("Foo*Dispose", false)]
        [DataRow("Bar.Foo", false)]
        [DataRow("Bar*", false)]
        [DataRow("Bar.", false)]
        [DataRow("Bar.*", false)]
        [DataRow("Foo", false)]
        [DataRow("Foo.", false)]
        [DataRow("*.*.*.*", false)]
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

        [TestMethod]
        [DataRow("Foo.MyType.ctor", true)]
        [DataRow("MyType.ctor", true)]
        [DataRow("Foo.MyType*.ctor", true)]
        [DataRow("Foo*.MyType*.ctor", true)]
        [DataRow("*.MyType*.ctor", true)]
        [DataRow("F*.My*ype*.ctor", true)]
        [DataRow("MyType*.ctor", true)]
        [DataRow("*MyType.ctor", true)]
        [DataRow("*MyType*.ctor", true)]
        [DataRow("*Type.ctor", true)]
        [DataRow("My*.ctor", true)]
        [DataRow("*.ctor", true)]
        [DataRow("*.*.ctor", true)]
        [DataRow("MyType.constructor", false)]
        [DataRow("Type.ctor", false)]
        [DataRow("Foo.ctor", false)]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

            SyntaxNode originalNode = FindEnclosingNode<ExpressionSyntax>(baseSyntaxTree, anchor);
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

            originalNode = FindEnclosingNode<StatementSyntax>(baseSyntaxTree, anchor);
            if (originalNode != null)
            {
                yield return (new Mutant
                {
                    Mutation = new Mutation
                    {
                        OriginalNode = originalNode,
                    }
                }, "Statement mutant");
            }

            originalNode = FindEnclosingNode<BlockSyntax>(baseSyntaxTree, anchor);
            if (originalNode != null)
            {
                yield return (new Mutant
                {
                    Mutation = new Mutation
                    {
                        OriginalNode = originalNode,
                    }
                }, "Block mutant");
            }

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
