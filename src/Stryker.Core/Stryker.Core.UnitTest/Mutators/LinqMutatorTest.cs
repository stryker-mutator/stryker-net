using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class LinqMutatorTest : TestBase
{
    /// <summary>
    ///     Generator for different Linq expressions
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    private static MemberAccessExpressionSyntax GenerateExpressions(string expression)
    {
        var tree = CSharpSyntaxTree.ParseText($@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            IEnumerable<string> Test = new[] {{}};

            Test.{expression}(_ => _!=null);
        }}
    }}
}}");
        var memberAccessExpression = tree.GetRoot()
            .DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>()
            .Single();

        return memberAccessExpression;
    }

    [TestMethod]
    public void ShouldBeMutationLevelStandard()
    {
        var target = new LinqMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    /// <summary>
    ///     Test method to check for correct mutation of different Linq Expression Mutations
    /// </summary>
    /// <param name="original"></param>
    /// <param name="expected"></param>
    [TestMethod]
    [DataRow(LinqExpression.FirstOrDefault, LinqExpression.First)]
    [DataRow(LinqExpression.First, LinqExpression.FirstOrDefault)]
    [DataRow(LinqExpression.SingleOrDefault, LinqExpression.Single)]
    [DataRow(LinqExpression.Single, LinqExpression.SingleOrDefault)]
    [DataRow(LinqExpression.Last, LinqExpression.First)]
    [DataRow(LinqExpression.All, LinqExpression.Any)]
    [DataRow(LinqExpression.Any, LinqExpression.All)]
    [DataRow(LinqExpression.Skip, LinqExpression.Take)]
    [DataRow(LinqExpression.Take, LinqExpression.Skip)]
    [DataRow(LinqExpression.SkipWhile, LinqExpression.TakeWhile)]
    [DataRow(LinqExpression.TakeWhile, LinqExpression.SkipWhile)]
    [DataRow(LinqExpression.Min, LinqExpression.Max)]
    [DataRow(LinqExpression.Max, LinqExpression.Min)]
    [DataRow(LinqExpression.Sum, LinqExpression.Max)]
    [DataRow(LinqExpression.Average, LinqExpression.Min)]
    [DataRow(LinqExpression.OrderBy, LinqExpression.OrderByDescending)]
    [DataRow(LinqExpression.OrderByDescending, LinqExpression.OrderBy)]
    [DataRow(LinqExpression.ThenBy, LinqExpression.ThenByDescending)]
    [DataRow(LinqExpression.ThenByDescending, LinqExpression.ThenBy)]
    [DataRow(LinqExpression.Reverse, LinqExpression.AsEnumerable)]
    [DataRow(LinqExpression.AsEnumerable, LinqExpression.Reverse)]
    [DataRow(LinqExpression.Union, LinqExpression.Intersect)]
    [DataRow(LinqExpression.Intersect, LinqExpression.Union)]
    [DataRow(LinqExpression.Concat, LinqExpression.Except)]
    [DataRow(LinqExpression.Except, LinqExpression.Concat)]
    [DataRow(LinqExpression.MinBy, LinqExpression.MaxBy)]
    [DataRow(LinqExpression.MaxBy, LinqExpression.MinBy)]
    [DataRow(LinqExpression.SkipLast, LinqExpression.TakeLast)]
    [DataRow(LinqExpression.TakeLast, LinqExpression.SkipLast)]
    [DataRow(LinqExpression.Order, LinqExpression.OrderDescending)]
    [DataRow(LinqExpression.OrderDescending, LinqExpression.Order)]
    [DataRow(LinqExpression.UnionBy, LinqExpression.IntersectBy)]
    [DataRow(LinqExpression.IntersectBy, LinqExpression.UnionBy)]
    [DataRow(LinqExpression.Append, LinqExpression.Prepend)]
    [DataRow(LinqExpression.Prepend, LinqExpression.Append)]
    public void ShouldMutate(LinqExpression original, LinqExpression expected)
    {
        var target = new LinqMutator();

        var expression = GenerateExpressions(original.ToString());

        var result = target.ApplyMutations(expression, null).ToList();

        var mutation = result.ShouldHaveSingleItem();
        var replacement = mutation.ReplacementNode.ShouldBeOfType<MemberAccessExpressionSyntax>();
        replacement.Name.Identifier.ValueText.ShouldBe(expected.ToString());

        mutation.DisplayName.ShouldBe($"Linq method mutation ({original}() to {expected}())");
    }

    /// <summary>
    ///     Test Method to check, if different expressions aren't mutated
    /// </summary>
    /// <param name="methodName"></param>
    [TestMethod]
    [DataRow("AllData")]
    [DataRow("PriceFirstOrDefault")]
    [DataRow("TakeEntry")]
    [DataRow("ShouldNotMutate")]
    [DataRow("WriteLine")]
    public void ShouldNotMutate(string methodName)
    {
        var target = new LinqMutator();

        var result = target.ApplyMutations(GenerateExpressions(methodName), null);

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateProperlyConditionalExpression()
    {
        var tree = CSharpSyntaxTree.ParseText(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<string> Test = new[] {};

            Test?.First.Second.Third.All(_ => _!=null);
        }
    }
}");
        var memberAccessExpression = tree
            .GetRoot()
            .DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>().Single(x => x.Name.ToString() == "All");
        var target = new LinqMutator();

        var result = target.ApplyMutations(memberAccessExpression, null);

        result.ShouldHaveSingleItem().ReplacementNode.ShouldBeOfType<MemberAccessExpressionSyntax>().Name.ToString().ShouldBe("Any");
    }

    [TestMethod]
    public void ShouldNotMutateAppendOnNonEnumerableReceiver()
    {
        var (semanticModel, memberAccess) = CreateSemanticModelForMemberAccess(
            """
            namespace TestApp
            {
                interface IResponseCookies { void Append(string key, string value); }
                class Program
                {
                    static void Run(IResponseCookies cookies) => cookies.Append("k", "v");
                }
            }
            """,
            "Append");

        var result = new LinqMutator().ApplyMutations(memberAccess, semanticModel);

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateAnyWithoutArgumentsBecauseAllRequiresPredicate()
    {
        // `Any` -> `All` mutation must be skipped when there are no arguments,
        // otherwise the mutant `items.All()` does not compile.
        var (semanticModel, memberAccess) = CreateSemanticModelForMemberAccess(
            """
            using System.Collections.Generic;
            using System.Linq;
            namespace TestApp
            {
                class Program
                {
                    static void Run(IEnumerable<int> items) { var x = items.Any(); }
                }
            }
            """,
            "Any");

        var result = new LinqMutator().ApplyMutations(memberAccess, semanticModel);

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateAppendOnEnumerableReceiver()
    {
        var (semanticModel, memberAccess) = CreateSemanticModelForMemberAccess(
            """
            using System.Collections.Generic;
            using System.Linq;
            namespace TestApp
            {
                class Program
                {
                    static void Run(IEnumerable<int> items) { var x = items.Append(1); }
                }
            }
            """,
            "Append");

        var result = new LinqMutator().ApplyMutations(memberAccess, semanticModel).ToList();

        result.ShouldHaveSingleItem();
    }

    private static (SemanticModel semanticModel, MemberAccessExpressionSyntax expression) CreateSemanticModelForMemberAccess(string source, string memberName)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create("TestAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(System.Collections.Generic.IEnumerable<>).Assembly.Location))
            .AddSyntaxTrees(syntaxTree);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var memberAccess = syntaxTree.GetRoot().DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>()
            .Single(x => x.Name.Identifier.ValueText == memberName);
        return (semanticModel, memberAccess);
    }
}
