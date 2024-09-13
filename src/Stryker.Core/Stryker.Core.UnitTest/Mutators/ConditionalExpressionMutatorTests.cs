using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class ConditionalExpressionMutatorTests : TestBase
{
    [TestMethod]
    public void ShouldBeMutationLevelStandard()
    {
        var target = new ConditionalExpressionMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [TestMethod]
    public void ShouldMutate_TwoMutations()
    {
        var target = new ConditionalExpressionMutator();
        var source = @"251 == 73 ? 1 : 0";
        var tree = CSharpSyntaxTree.ParseText(source);
        var originalNode = tree.GetRoot().DescendantNodes().OfType<ConditionalExpressionSyntax>().Single();

        var result = target.ApplyMutations(originalNode, null).ToList();

        result.Count.ShouldBe(2, "Two mutations should have been made");
        result.First().ShouldBeOfType<Mutation>().ReplacementNode.ShouldBeOfType<ParenthesizedExpressionSyntax>().Expression.ShouldBeOfType<ConditionalExpressionSyntax>().Condition.Kind().ShouldBe(SyntaxKind.TrueLiteralExpression);
        result.Last().ShouldBeOfType<Mutation>().ReplacementNode.ShouldBeOfType<ParenthesizedExpressionSyntax>().Expression.ShouldBeOfType<ConditionalExpressionSyntax>().Condition.Kind().ShouldBe(SyntaxKind.FalseLiteralExpression);
    }

    [TestMethod]
    public void ShouldMutate_DoNotTouchBranches()
    {
        var target = new ConditionalExpressionMutator();
        var source = "251 == 73 ? 1 : 0";
        var tree = CSharpSyntaxTree.ParseText(source);
        var originalNode = tree.GetRoot().DescendantNodes().OfType<ConditionalExpressionSyntax>().Single();

        var result = target.ApplyMutations(originalNode, null).ToList();

        foreach (var mutation in result)
        {
            var pes = mutation.ReplacementNode.ShouldBeOfType<ParenthesizedExpressionSyntax>();
            var ces = pes.Expression.ShouldBeOfType<ConditionalExpressionSyntax>();
            ces.WhenTrue.IsEquivalentTo(originalNode.WhenTrue).ShouldBeTrue();
            ces.WhenFalse.IsEquivalentTo(originalNode.WhenFalse).ShouldBeTrue();
        }
    }

    [TestMethod]
    public void ShouldNotMutateDeclarationPatterns()
    {
        var target = new ConditionalExpressionMutator();
        var source = "var y = x is object result ? result.ToString() : null;";
        var tree = CSharpSyntaxTree.ParseText(source);

        var expressionSyntax = tree.GetRoot().DescendantNodes().OfType<ConditionalExpressionSyntax>().Single();
        var result = target.ApplyMutations(expressionSyntax, null).ToList();

        result.ShouldBeEmpty();
    }
}
