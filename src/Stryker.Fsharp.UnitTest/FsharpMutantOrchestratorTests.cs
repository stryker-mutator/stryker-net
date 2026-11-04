using Shouldly;
using Stryker.Abstractions;
using Stryker.Fsharp.Mutants;
using Stryker.Fsharp.Mutators;
using Stryker.Fsharp.UnitTest.Fixtures;

namespace Stryker.Fsharp.UnitTest;

[TestClass]
public class FsharpMutantOrchestratorTests
{
    private const string Ns = "StrykerTest";

    [TestMethod]
    public void BooleanMutator_TrueLiteral_YieldsFalseReplacement()
    {
        var source = "module M\nlet f () = true\n";
        var leaf = FsharpParse.Leaf(source);
        var expr = FsharpParse.FirstBindingExpression(leaf);

        var mutation = new BooleanMutator().ApplyMutations(expr, source).Single();

        mutation.OriginalText.ShouldBe("true");
        mutation.ReplacementText.ShouldBe("false");
        mutation.Type.ShouldBe(Mutator.Boolean);
        // FCS range is (line 2, col 11)-(line 2, col 15); columns are 1-based in MutationLocation
        mutation.Location.StartLine.ShouldBe(2);
        mutation.Location.StartColumn.ShouldBe(12);
        mutation.Location.EndColumn.ShouldBe(16);
        mutation.Location.StartOffset.ShouldBe(source.IndexOf("true", StringComparison.Ordinal));
        mutation.Location.EndOffset.ShouldBe(source.IndexOf("true", StringComparison.Ordinal) + 4);
    }

    [TestMethod]
    public void BooleanMutator_FalseLiteral_YieldsTrueReplacement()
    {
        var source = "module M\nlet f () = false\n";
        var expr = FsharpParse.FirstBindingExpression(FsharpParse.Leaf(source));

        new BooleanMutator().ApplyMutations(expr, source).Single().ReplacementText.ShouldBe("true");
    }

    [TestMethod]
    public void Orchestrator_SplicesSingleMutant_ParenthesisedWithId()
    {
        var source = "module M\nlet f () = true\n";
        var leaf = FsharpParse.Leaf(source);

        var count = new FsharpMutantOrchestrator(Ns).Mutate(leaf);

        count.ShouldBe(1);
        leaf.Mutants.Single().Id.ShouldBe(0);
        leaf.Mutants.Single().Mutation.ReplacementText.ShouldBe("false");
        FsharpMutantOrchestrator.ApplySplices(source, leaf.Splices, Ns)
            .ShouldBe("module M\nlet f () = (if StrykerTest.MutantControl.IsActive 0 then false else true)\n");
    }

    [TestMethod]
    public void Orchestrator_Quotation_YieldsNoMutants()
    {
        var leaf = FsharpParse.Leaf("module M\nlet q = <@ true @>\n");

        new FsharpMutantOrchestrator(Ns).Mutate(leaf).ShouldBe(0);

        leaf.Mutants.ShouldBeEmpty();
        leaf.Splices.ShouldBeEmpty();
    }

    [TestMethod]
    public void Orchestrator_SeveralMutants_GetSequentialIds_AndSpliceRightToLeft()
    {
        var source = "module M\nlet f () = if true then false else true\n";
        var leaf = FsharpParse.Leaf(source);

        new FsharpMutantOrchestrator(Ns).Mutate(leaf).ShouldBe(3);

        leaf.Mutants.Select(m => m.Id).ShouldBe([0, 1, 2]);
        FsharpMutantOrchestrator.ApplySplices(source, leaf.Splices, Ns).ShouldBe(
            "module M\nlet f () = if (if StrykerTest.MutantControl.IsActive 0 then false else true) then (if StrykerTest.MutantControl.IsActive 1 then true else false) else (if StrykerTest.MutantControl.IsActive 2 then false else true)\n");
    }

    [TestMethod]
    public void Orchestrator_NestedMutants_SpliceInnerFirst_ExactText()
    {
        const string source = "module M\nlet f a = a || true\n";
        var outer = SpliceOf(source, "a || true", 0, "a && true");
        var inner = SpliceOf(source, "true", 1, "false");

        var expected = "module M\nlet f a = (if StrykerTest.MutantControl.IsActive 0 then a && (if StrykerTest.MutantControl.IsActive 1 then false else true) else a || (if StrykerTest.MutantControl.IsActive 1 then false else true))\n";
        FsharpMutantOrchestrator.ApplySplices(source, [inner, outer], Ns).ShouldBe(expected);
        FsharpMutantOrchestrator.ApplySplices(source, [outer, inner], Ns).ShouldBe(expected);
    }

    [TestMethod]
    public void Orchestrator_NestedMutants_PlacedInnermostFirstInEveryBranch()
    {
        const string source = "module M\nlet f a = a || true\n";
        var applied = FsharpMutantOrchestrator.ApplySplicesTracked(source, [SpliceOf(source, "a || true", 0, "a && true"), SpliceOf(source, "true", 1, "false")], Ns);

        foreach (var placed in applied.Placed)
        {
            applied.Text[placed.StartOffset].ShouldBe('(');
            applied.Text[placed.EndOffset - 1].ShouldBe(')');
        }

        applied.Placed.Count(p => p.MutantId == 1).ShouldBe(2, "the inner splice sits in both branches of the outer one");
        var outerPlaced = applied.Placed.Single(p => p.MutantId == 0);
        applied.Placed.Where(p => p.MutantId == 1).ShouldAllBe(p => p.StartOffset > outerPlaced.StartOffset && p.EndOffset < outerPlaced.EndOffset);
    }

    [TestMethod]
    public void Orchestrator_PartiallyOverlappingSplices_AreRejected()
    {
        const string source = "module M\nlet f a = a || true\n";

        Should.Throw<InvalidOperationException>(() => FsharpMutantOrchestrator.ApplySplices(source,
            [SpliceOf(source, "a ||", 0, "a &&"), SpliceOf(source, "|| true", 1, "&& false")], Ns));
    }

    internal static TextSplice SpliceOf(string source, string text, int id, string mutated)
    {
        var start = source.LastIndexOf(text, StringComparison.Ordinal);
        return new TextSplice(id, new MutationLocation("/proj/M.fs", 1, 1, 1, 1, start, start + text.Length), text, mutated);
    }
}
