using Shouldly;
using Stryker.Abstractions;
using Stryker.Fsharp.Mutants;
using Stryker.Fsharp.Mutators;
using Stryker.Fsharp.UnitTest.Fixtures;

namespace Stryker.Fsharp.UnitTest;

[TestClass]
public class FsharpEqualityMutatorTests
{
    private const string Ns = "StrykerTest";

    [TestMethod]
    public void EqualityMutator_Equals_YieldsNotEquals()
    {
        var source = "module M\nlet f a b = a = b\n";
        var expr = FsharpParse.FirstBindingExpression(FsharpParse.Leaf(source));

        var mutation = new EqualityOperatorMutator().ApplyMutations(expr, source).Single();

        mutation.OriginalText.ShouldBe("a = b");
        mutation.ReplacementText.ShouldBe("a <> b");
        mutation.Type.ShouldBe(Mutator.Equality);
        mutation.Location.StartOffset.ShouldBe(source.IndexOf("a = b", StringComparison.Ordinal));
        mutation.Location.EndOffset.ShouldBe(source.IndexOf("a = b", StringComparison.Ordinal) + 5);
    }

    [TestMethod]
    [DataRow("=", new[] { "<>" })]
    [DataRow("<>", new[] { "=" })]
    [DataRow("<", new[] { ">", "<=" })]
    [DataRow("<=", new[] { ">", "<" })]
    [DataRow(">", new[] { "<", ">=" })]
    [DataRow(">=", new[] { "<", ">" })]
    public void EqualityMutator_Pair_YieldsExactReplacement(string original, string[] replacements)
    {
        var source = $"module M\nlet f a b = a {original} b\n";
        var expr = FsharpParse.FirstBindingExpression(FsharpParse.Leaf(source));

        new EqualityOperatorMutator().ApplyMutations(expr, source).Select(m => m.ReplacementText)
            .ShouldBe(replacements.Select(r => $"a {r} b"));
    }

    [TestMethod]
    public void EqualityMutator_ReportCount_OnFixtureFile()
    {
        var source = "module M\nlet a x y = x = y\nlet b x y = x <> y\nlet c x y = x < y\nlet d x y = x <= y\nlet e x y = x > y\nlet f x y = x >= y\nlet g x = x\nlet h = List.exists ((=) 1)\n";
        var leaf = FsharpParse.Leaf(source);

        // 1 + 1 + 2 + 2 + 2 + 2; plain bindings and the (=) section are not mutated
        new FsharpMutantOrchestrator(Ns, [new EqualityOperatorMutator()]).Mutate(leaf).ShouldBe(10);
        leaf.Mutants.Select(m => m.Mutation.ReplacementText).ShouldBe(
            ["x <> y", "x = y", "x > y", "x <= y", "x > y", "x < y", "x < y", "x >= y", "x < y", "x > y"], ignoreOrder: true);
    }

    [TestMethod]
    public void EqualityMutator_CompiledMutant_ChangesResult()
    {
        using var project = new MutatedFsharpProject("module Lib\nlet cmp (a: int) (b: int) = a < b\n");
        var toLessOrEqual = project.Leaf.Mutants.Single(m => m.Mutation.ReplacementText == "a <= b");
        var toGreater = project.Leaf.Mutants.Single(m => m.Mutation.ReplacementText == "a > b");
        var loaded = project.Load();

        loaded.Set("ActiveMutant", -1);
        loaded.Call("Lib", "cmp", 3, 3).ShouldBe(false);
        loaded.Set("ActiveMutant", toLessOrEqual.Id);
        loaded.Call("Lib", "cmp", 3, 3).ShouldBe(true);
        loaded.Set("ActiveMutant", toGreater.Id);
        loaded.Call("Lib", "cmp", 3, 2).ShouldBe(true);
    }

    [TestMethod]
    public void Orchestrator_MutationLevelBasic_ExcludesAboveBasicMutators()
    {
        var source = "module M\nlet f a b = a + b = 1\nlet g = true\n";
        IFsharpMutator[] all = [new ArithmeticOperatorMutator(), new EqualityOperatorMutator(), new BooleanMutator(), new CompleteOnlyMutator()];

        var basic = FsharpParse.Leaf(source);
        new FsharpMutantOrchestrator(Ns, all, MutationLevel.Basic).Mutate(basic).ShouldBe(2);
        basic.Mutants.Select(m => m.Mutation.Type).ShouldBe([Mutator.Equality, Mutator.Arithmetic], ignoreOrder: true);

        var standard = FsharpParse.Leaf(source);
        new FsharpMutantOrchestrator(Ns, all, MutationLevel.Standard).Mutate(standard).ShouldBe(3);

        var complete = FsharpParse.Leaf(source);
        new FsharpMutantOrchestrator(Ns, all, MutationLevel.Complete).Mutate(complete).ShouldBeGreaterThan(3);
        complete.Mutants.ShouldContain(m => m.Mutation.Type == Mutator.Statement);
    }

    private sealed class CompleteOnlyMutator : IFsharpMutator
    {
        public MutationLevel MutationLevel => MutationLevel.Complete;

        public IEnumerable<Mutation> ApplyMutations(FSharp.Compiler.Syntax.SynExpr expr, string sourceText)
        {
            if (expr is FSharp.Compiler.Syntax.SynExpr.Const)
            {
                var location = FsharpLocations.ToLocation(expr.Range, sourceText);
                yield return new Mutation
                {
                    Location = location,
                    OriginalText = sourceText[location.StartOffset..location.EndOffset],
                    ReplacementText = "0",
                    Type = Mutator.Statement,
                    DisplayName = "complete-only",
                };
            }
        }
    }
}
