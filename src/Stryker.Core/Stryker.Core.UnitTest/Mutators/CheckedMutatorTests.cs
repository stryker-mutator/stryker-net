using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class CheckedMutatorTests : TestBase
{
    [TestMethod]
    public void ShouldBeMutationLevelComplete()
    {
        var target = new CheckedMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [TestMethod]
    [DataRow(SyntaxKind.CheckedExpression, "4 + 2", SyntaxKind.AddExpression)]
    public void ShouldMutate(SyntaxKind original, string expression, SyntaxKind expected)
    {
        var target = new CheckedMutator();

        var es = SyntaxFactory.ParseExpression(expression);
        var result = target.ApplyMutations(SyntaxFactory.CheckedExpression(original, es), null).ToList();

        var mutation = result.ShouldHaveSingleItem();

        mutation.ReplacementNode.IsKind(expected).ShouldBeTrue();
        mutation.DisplayName.ShouldBe("Remove checked expression");
    }

    [TestMethod]
    [DataRow(SyntaxKind.UncheckedExpression)]
    public void ShouldNotMutate(SyntaxKind original)
    {
        var target = new CheckedMutator();

        var es = SyntaxFactory.ParseExpression("4 + 2");
        var result = target.ApplyMutations(SyntaxFactory.CheckedExpression(original, es), null).ToList();

        result.ShouldBeEmpty();
    }
}
