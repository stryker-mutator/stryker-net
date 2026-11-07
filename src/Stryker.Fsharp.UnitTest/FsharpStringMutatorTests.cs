using Shouldly;
using Stryker.Abstractions;
using Stryker.Fsharp.Mutants;
using Stryker.Fsharp.Mutators;
using Stryker.Fsharp.UnitTest.Fixtures;

namespace Stryker.Fsharp.UnitTest;

[TestClass]
public class FsharpStringMutatorTests
{
    private const string Ns = "StrykerTest";

    private static List<Mutation> Mutations(string source)
    {
        var leaf = FsharpParse.Leaf(source);
        new FsharpMutantOrchestrator(Ns, [new StringLiteralMutator()]).Mutate(leaf);
        return leaf.Mutants.Select(m => m.Mutation).ToList();
    }

    [TestMethod]
    public void StringMutator_NonEmptyLiteral_YieldsEmpty()
    {
        var source = "module M\nlet f = \"abc\"\n";
        var expr = FsharpParse.FirstBindingExpression(FsharpParse.Leaf(source));

        var mutation = new StringLiteralMutator().ApplyMutations(expr, source).Single();

        mutation.OriginalText.ShouldBe("\"abc\"");
        mutation.ReplacementText.ShouldBe("\"\"");
        mutation.Type.ShouldBe(Mutator.String);
        new StringLiteralMutator().MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [TestMethod]
    public void StringMutator_EmptyLiteral_YieldsStrykerWasHere()
    {
        var source = "module M\nlet f = \"\"\n";
        var expr = FsharpParse.FirstBindingExpression(FsharpParse.Leaf(source));

        new StringLiteralMutator().ApplyMutations(expr, source).Single().ReplacementText.ShouldBe("\"Stryker was here!\"");
    }

    [TestMethod]
    [DataRow("@\"a\\b\"", "\"\"")]
    [DataRow("\"\"\"a \"q\" b\"\"\"", "\"\"")]
    public void StringMutator_VerbatimAndTripleQuoted_ReplacedWholeWithPlainEmpty(string literal, string replacement)
    {
        var mutation = Mutations($"module M\nlet f = {literal}\n").Single();

        mutation.OriginalText.ShouldBe(literal);
        mutation.ReplacementText.ShouldBe(replacement);
    }

    [TestMethod]
    public void StringMutator_StringEmptyMember_YieldsStrykerWasHere()
    {
        var mutations = Mutations("module M\nlet a = String.Empty\nlet b = System.String.Empty\nlet c = Foo.Empty\n");

        mutations.Select(m => (m.OriginalText, m.ReplacementText)).ShouldBe(
            [("String.Empty", "\"Stryker was here!\""), ("System.String.Empty", "\"Stryker was here!\"")]);
    }

    [TestMethod]
    public void StringMutator_LiteralAttribute_IsSkipped()
    {
        Mutations("module M\n[<Literal>]\nlet X = \"c\"\nlet y = \"d\"\n").Select(m => m.OriginalText).ShouldBe(["\"d\""]);
    }

    [TestMethod]
    public void StringMutator_AttributeArgument_IsSkipped()
    {
        Mutations("module M\nopen System\n[<Obsolete(\"msg\")>]\nlet f x = x\n").ShouldBeEmpty();
    }

    [TestMethod]
    public void StringMutator_InterpolatedString_HandledWholeOrSkipped()
    {
        // rule: interpolated strings are skipped entirely, including their holes' quotes
        Mutations("module M\nlet n = 1\nlet f = $\"a {n} b\"\n").ShouldBeEmpty();
    }

    [TestMethod]
    public void StringMutator_ReportCount_OnFixtureFile()
    {
        var source = "module M\nopen System\n[<Literal>]\nlet L = \"lit\"\n[<Obsolete(\"m\")>]\nlet a x = \"abc\"\nlet b x = \"\"\nlet c x = String.Empty\nlet d x = $\"i {x}\"\nlet e x = x\n";
        var leaf = FsharpParse.Leaf(source);

        new FsharpMutantOrchestrator(Ns, [new StringLiteralMutator()]).Mutate(leaf).ShouldBe(3);
        leaf.Mutants.Select(m => m.Mutation.ReplacementText).ShouldBe(["\"\"", "\"Stryker was here!\"", "\"Stryker was here!\""], ignoreOrder: true);
    }

    [TestMethod]
    public void StringMutator_CompiledMutant_ReturnsEmptyString()
    {
        using var project = new MutatedFsharpProject("module Lib\nlet greet (n: int) = \"hello\"\n");
        var mutant = project.Leaf.Mutants.Single(m => m.Mutation.Type == Mutator.String);
        var loaded = project.Load();

        loaded.Set("ActiveMutant", -1);
        loaded.Call("Lib", "greet", 1).ShouldBe("hello");
        loaded.Set("ActiveMutant", mutant.Id);
        loaded.Call("Lib", "greet", 1).ShouldBe("");
    }
}
