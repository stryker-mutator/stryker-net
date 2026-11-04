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
}
