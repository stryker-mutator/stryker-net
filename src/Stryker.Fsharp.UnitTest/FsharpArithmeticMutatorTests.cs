using Shouldly;
using Stryker.Abstractions;
using Stryker.Fsharp.Mutants;
using Stryker.Fsharp.Mutators;
using Stryker.Fsharp.UnitTest.Fixtures;

namespace Stryker.Fsharp.UnitTest;

[TestClass]
public class FsharpArithmeticMutatorTests
{
    private const string Ns = "StrykerTest";

    [TestMethod]
    public void ArithmeticMutator_Addition_YieldsSubtraction()
    {
        var source = "module M\nlet f a b = a + b\n";
        var expr = FsharpParse.FirstBindingExpression(FsharpParse.Leaf(source));

        var mutation = new ArithmeticOperatorMutator().ApplyMutations(expr, source).Single();

        mutation.OriginalText.ShouldBe("a + b");
        mutation.ReplacementText.ShouldBe("a - b");
        mutation.Type.ShouldBe(Mutator.Arithmetic);
        mutation.Location.StartOffset.ShouldBe(source.IndexOf("a + b", StringComparison.Ordinal));
        mutation.Location.EndOffset.ShouldBe(source.IndexOf("a + b", StringComparison.Ordinal) + 5);
    }

    [TestMethod]
    [DataRow("+", "-")]
    [DataRow("-", "+")]
    [DataRow("*", "/")]
    [DataRow("/", "*")]
    [DataRow("%", "*")]
    public void ArithmeticMutator_Pair_YieldsExactReplacement(string original, string replacement)
    {
        var source = $"module M\nlet f a b = a {original} b\n";
        var expr = FsharpParse.FirstBindingExpression(FsharpParse.Leaf(source));

        new ArithmeticOperatorMutator().ApplyMutations(expr, source).Single().ReplacementText.ShouldBe($"a {replacement} b");
    }

    [TestMethod]
    public void ArithmeticMutator_OperatorSection_IsNotMutated()
    {
        var source = "module M\nlet s = List.fold (+) 0\n";
        var leaf = FsharpParse.Leaf(source);

        new FsharpMutantOrchestrator(Ns, [new ArithmeticOperatorMutator()]).Mutate(leaf).ShouldBe(0);
    }

    [TestMethod]
    public void ArithmeticMutator_UnaryMinus_IsNotMutated()
    {
        var source = "module M\nlet f a = -a\n";
        var leaf = FsharpParse.Leaf(source);

        new FsharpMutantOrchestrator(Ns, [new ArithmeticOperatorMutator()]).Mutate(leaf).ShouldBe(0);
    }

    [TestMethod]
    public void ArithmeticMutator_ReportCount_OnFixtureFile()
    {
        var source = "module M\nlet a x y = x + y\nlet b x y = x * y - 1\nlet c x = x / 2 % 3\nlet d (x: string) = x + \"s\"\nlet e x = -x\nlet f = List.fold (+) 0\n";
        var leaf = FsharpParse.Leaf(source);

        // 1 + 2 + 2 + 1 (nested operators give one mutant each); string concatenation is mutated too: rollback removes it at compile time
        new FsharpMutantOrchestrator(Ns, [new ArithmeticOperatorMutator()]).Mutate(leaf).ShouldBe(6);
        leaf.Mutants.Select(m => m.Mutation.ReplacementText).ShouldBe(
            ["x - y", "x * y + 1", "x / y", "x / 2 * 3", "x * 2", "x - \"s\""], ignoreOrder: true);
    }

    [TestMethod]
    public void ArithmeticMutator_NestedChain_ComposesInnerFirst()
    {
        var source = "module M\nlet f a b c = a + b + c\n";
        var leaf = FsharpParse.Leaf(source);
        new FsharpMutantOrchestrator(Ns, [new ArithmeticOperatorMutator()]).Mutate(leaf).ShouldBe(2);

        FsharpMutantOrchestrator.ApplySplices(source, leaf.Splices, Ns)
            .ShouldBe("module M\nlet f a b c = (if StrykerTest.MutantControl.IsActive 0 then (if StrykerTest.MutantControl.IsActive 1 then a - b else a + b) - c else (if StrykerTest.MutantControl.IsActive 1 then a - b else a + b) + c)\n");
    }

    [TestMethod]
    public void ArithmeticMutator_CompiledMutant_ChangesResult()
    {
        using var project = new MutatedFsharpProject("module Lib\nlet calc (a: int) (b: int) = a + b\n");
        var mutant = project.Leaf.Mutants.Single(m => m.Mutation.Type == Mutator.Arithmetic);
        var loaded = project.Load();

        loaded.Set("ActiveMutant", -1);
        loaded.Call("Lib", "calc", 7, 3).ShouldBe(10);
        loaded.Set("ActiveMutant", mutant.Id);
        loaded.Call("Lib", "calc", 7, 3).ShouldBe(4);
    }
}
