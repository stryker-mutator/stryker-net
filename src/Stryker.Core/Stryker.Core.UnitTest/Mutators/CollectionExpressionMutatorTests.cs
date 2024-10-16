using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class CollectionExpressionMutatorTests : TestBase
{
    [TestMethod]
    public void ShouldBeMutationLevelAdvanced()
    {
        var target = new CollectionExpressionMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Advanced);
    }

    [TestMethod]
    [DataRow("[]")]
    [DataRow("[ ]")]
    [DataRow("[           ]")]
    [DataRow("[ /* Comment */ ]")]
    public void ShouldAddValueToEmptyCollectionExpression(string expression)
    {
        var expressionSyntax = SyntaxFactory.ParseExpression(expression) as CollectionExpressionSyntax;
        var target = new CollectionExpressionMutator();
        var result = target.ApplyMutations(expressionSyntax, null);
        result.Count().ShouldBe(2);

        foreach (var mutation in result)
        {
            mutation.DisplayName.ShouldBe("Collection expression mutation");
            var replacement = mutation.ReplacementNode.ShouldBeOfType<CollectionExpressionSyntax>();
            replacement.Elements.ShouldNotBeEmpty();
        }
    }

    [TestMethod]
    [DataRow("[1, 2, 3]")]
    [DataRow("[-1, 3]")]
    [DataRow("[1, .. abc, 3]")]
    [DataRow("[..abc]")]
    public void ShouldRemoveValuesFromCollectionExpression(string expression)
    {
        var expressionSyntax = SyntaxFactory.ParseExpression(expression) as CollectionExpressionSyntax;
        var target = new CollectionExpressionMutator();
        var result = target.ApplyMutations(expressionSyntax, null);
        var mutation = result.ShouldHaveSingleItem();
        mutation.DisplayName.ShouldBe("Collection expression mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<CollectionExpressionSyntax>();
        replacement.Elements.ShouldBeEmpty();
    }
}
