using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;

namespace Stryker.Core.UnitTest.MutationTest;

[TestClass]
public class TimeoutCommentInserterTests : TestBase
{
    private const string Reason = TimeoutCommentInserter.Reason;

    private static (string Result, IReadOnlyList<IReadOnlyMutant> Unplaced) Insert(string source, params (string Text, Mutator Type)[] mutations)
    {
        var root = CSharpSyntaxTree.ParseText(source).GetRoot();
        var mutants = mutations.Select(m =>
        {
            var node = root.DescendantNodes().First(n => n.ToString() == m.Text);
            return (IReadOnlyMutant)new Mutant { Mutation = new Mutation { OriginalNode = node, Type = m.Type } };
        }).ToList();

        var result = TimeoutCommentInserter.Insert(root, mutants, out var unplaced);
        return (result, unplaced);
    }

    [TestMethod]
    public void ShouldAddCommentAboveStatementKeepingIndentation()
    {
        var (result, unplaced) = Insert(
            "class C\n{\n    void M(int x)\n    {\n        x = x + 1;\n    }\n}\n",
            ("x + 1", Mutator.Arithmetic));

        result.ShouldBe($"class C\n{{\n    void M(int x)\n    {{\n        // Stryker disable once Arithmetic: {Reason}\n        x = x + 1;\n    }}\n}}\n");
        unplaced.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldCombineMutatorsOfTheSameStatementInOneComment()
    {
        var (result, _) = Insert(
            "class C\n{\n    int M(int x)\n    {\n        return x + 1 - 2;\n    }\n}\n",
            ("x + 1", Mutator.Arithmetic),
            ("x + 1 - 2", Mutator.Arithmetic),
            ("return x + 1 - 2;", Mutator.Statement));

        result.ShouldContain($"// Stryker disable once Arithmetic,Statement: {Reason}");
        result.Split("Stryker disable").Length.ShouldBe(2);
    }

    [TestMethod]
    public void ShouldUseWindowsLineEndingsWhenSourceUsesThem()
    {
        var (result, _) = Insert(
            "class C\r\n{\r\n    void M(int x)\r\n    {\r\n        x++;\r\n    }\r\n}\r\n",
            ("x++", Mutator.Update));

        result.ShouldContain($"        // Stryker disable once Update: {Reason}\r\n        x++;");
        result.Replace("\r\n", "").ShouldNotContain("\n");
    }

    [TestMethod]
    public void ShouldPlaceCommentBeforeEmbeddedStatement()
    {
        var (result, _) = Insert(
            "class C\n{\n    void M(int x)\n    {\n        if (x > 0)\n            x++;\n    }\n}\n",
            ("x++", Mutator.Update));

        result.ShouldContain($"        if (x > 0)\n            // Stryker disable once Update: {Reason}\n            x++;");
    }

    [TestMethod]
    public void ShouldPlaceCommentOnConditionOfIfStatement()
    {
        var (result, _) = Insert(
            "class C\n{\n    void M(int x)\n    {\n        if (x > 0)\n        {\n            x++;\n        }\n    }\n}\n",
            ("x > 0", Mutator.Equality));

        result.ShouldContain($"        // Stryker disable once Equality: {Reason}\n        if (x > 0)");
    }

    [TestMethod]
    public void ShouldPlaceCommentAfterElseWhenIfFollowsElse()
    {
        var (result, _) = Insert(
            "class C\n{\n    void M(int x)\n    {\n        if (x > 0) { }\n        else if (x < 0) { }\n    }\n}\n",
            ("x < 0", Mutator.Equality));

        result.ShouldContain($"else // Stryker disable once Equality: {Reason}\n        if (x < 0)");
        CSharpSyntaxTree.ParseText(result).GetDiagnostics().ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldPlaceCommentAboveMemberForExpressionBodiedMembers()
    {
        var (result, _) = Insert(
            "class C\n{\n    public int P => 1 + 2;\n}\n",
            ("1 + 2", Mutator.Arithmetic));

        result.ShouldContain($"    // Stryker disable once Arithmetic: {Reason}\n    public int P => 1 + 2;");
    }

    [TestMethod]
    public void ShouldPlaceCommentAboveFieldInitializer()
    {
        var (result, _) = Insert(
            "class C\n{\n    private readonly int _f = 1 + 2;\n}\n",
            ("1 + 2", Mutator.Arithmetic));

        result.ShouldContain($"    // Stryker disable once Arithmetic: {Reason}\n    private readonly int _f = 1 + 2;");
    }

    [TestMethod]
    public void ShouldKeepExistingTriviaAndProduceValidCode()
    {
        var (result, _) = Insert(
            "class C\n{\n    void M(int x)\n    {\n        // existing comment\n\n        x = x + 1; // trailing\n    }\n}\n",
            ("x + 1", Mutator.Arithmetic));

        result.ShouldContain($"// existing comment\n\n        // Stryker disable once Arithmetic: {Reason}\n        x = x + 1; // trailing");
        CSharpSyntaxTree.ParseText(result).GetDiagnostics().ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldReturnNullWhenNoMutants()
    {
        var root = CSharpSyntaxTree.ParseText("class C { }").GetRoot();

        TimeoutCommentInserter.Insert(root, [], out var unplaced).ShouldBeNull();
        unplaced.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldReportMutantsOutsideTheSyntaxTree()
    {
        var root = CSharpSyntaxTree.ParseText("class C { }").GetRoot();
        var otherNode = CSharpSyntaxTree.ParseText("class D { void M() { var xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx = 1; } }").GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().First();
        var mutant = new Mutant { Mutation = new Mutation { OriginalNode = otherNode, Type = Mutator.Arithmetic } };

        TimeoutCommentInserter.Insert(root, [mutant], out var unplaced).ShouldBeNull();
        unplaced.ShouldHaveSingleItem();
    }
}
