using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class RegexMutatorTest : TestBase
{
    [TestMethod]
    public void ShouldBeMutationLevelAdvanced()
    {
        var target = new RegexMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Advanced);
    }

    [TestMethod]
    public void ShouldMutateStringLiteralInRegexConstructor()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex(@\"^abc\")") as ObjectCreationExpressionSyntax;
        var target = new RegexMutator();

        var result = target.ApplyMutations(objectCreationExpression, null);

        var mutation = result.ShouldHaveSingleItem();

        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        var replacementNode = mutation.ReplacementNode as InvocationExpressionSyntax;
        replacementNode.ShouldNotBeNull();
        var memberAccess = replacementNode.Expression as MemberAccessExpressionSyntax;
        memberAccess.ShouldBeNull(); // Ensure it's a simple invocation, not a member access
        var identifier = (replacementNode.Expression as IdentifierNameSyntax)?.Identifier.Text;
        identifier.ShouldBe("Regex");
        var argument = replacementNode.ArgumentList.Arguments.First().Expression as LiteralExpressionSyntax;
        argument.ShouldNotBeNull();
        argument.Token.ValueText.ShouldBe("abc");
    }

    [TestMethod]
    public void ShouldMutateStringLiteralInRegexConstructorWithFullName()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new System.Text.RegularExpressions.Regex(@\"^abc\")") as ObjectCreationExpressionSyntax;
        var target = new RegexMutator();

        var result = target.ApplyMutations(objectCreationExpression, null);

        var mutation = result.ShouldHaveSingleItem();

        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        var replacementNode = mutation.ReplacementNode as ObjectCreationExpressionSyntax;
        replacementNode.ShouldNotBeNull();
        replacementNode.Type.ToString().ShouldBe("System.Text.RegularExpressions.Regex");
        var argument = replacementNode.ArgumentList.Arguments.FirstOrDefault();
        argument.ShouldNotBeNull();
        argument.ToString().ShouldBe("\"abc\"");
    }


    [TestMethod]
    public void ShouldNotMutateRegexWithoutParameters()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex()") as ObjectCreationExpressionSyntax;
        var target = new RegexMutator();
        var result = target.ApplyMutations(objectCreationExpression, null);

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateStringLiteralInOtherConstructor()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Other(@\"^abc\")") as ObjectCreationExpressionSyntax;
        var target = new RegexMutator();
        var result = target.ApplyMutations(objectCreationExpression, null);

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateStringLiteralMultipleTimes()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex(@\"^abc$\")") as ObjectCreationExpressionSyntax;
        var target = new RegexMutator();

        var result = target.ApplyMutations(objectCreationExpression, null);

        result.Count().ShouldBe(2);
        result.ShouldAllBe(mutant => mutant.DisplayName == "Regex anchor removal mutation");

        var firstReplacement = result.First().ReplacementNode as ObjectCreationExpressionSyntax;
        firstReplacement.ShouldNotBeNull();
        firstReplacement.ArgumentList.Arguments.Count.ShouldBe(1);
        firstReplacement.ArgumentList.Arguments[0].Expression.ShouldBeOfType<LiteralExpressionSyntax>();
        ((LiteralExpressionSyntax)firstReplacement.ArgumentList.Arguments[0].Expression).Token.ValueText.ShouldBe("abc$");

        var lastReplacement = result.Last().ReplacementNode as ObjectCreationExpressionSyntax;
        lastReplacement.ShouldNotBeNull();
        lastReplacement.ArgumentList.Arguments.Count.ShouldBe(1);
        lastReplacement.ArgumentList.Arguments[0].Expression.ShouldBeOfType<LiteralExpressionSyntax>();
        ((LiteralExpressionSyntax)lastReplacement.ArgumentList.Arguments[0].Expression).Token.ValueText.ShouldBe("^abc");
    }

    [TestMethod]
    public void ShouldMutateStringLiteralAsNamedArgumentPatternInRegexConstructor()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex(options: RegexOptions.None, pattern: @\"^abc\")") as ObjectCreationExpressionSyntax;
        var target = new RegexMutator();

        var result = target.ApplyMutations(objectCreationExpression, null);

        var mutation = result.ShouldHaveSingleItem();

        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        var replacementNode = mutation.ReplacementNode as InvocationExpressionSyntax;
        replacementNode.ShouldNotBeNull();

        var argumentList = replacementNode.ArgumentList;
        argumentList.ShouldNotBeNull();
        argumentList.Arguments.Count.ShouldBe(2);

        var patternArgument = argumentList.Arguments.First(arg => arg.NameColon?.Name.Identifier.Text == "pattern");
        patternArgument.ShouldNotBeNull();
        patternArgument.Expression.ToString().ShouldBe("\"abc\"");

        var optionsArgument = argumentList.Arguments.First(arg => arg.NameColon?.Name.Identifier.Text == "options");
        optionsArgument.ShouldNotBeNull();
        optionsArgument.Expression.ToString().ShouldBe("RegexOptions.None");
    }
}
