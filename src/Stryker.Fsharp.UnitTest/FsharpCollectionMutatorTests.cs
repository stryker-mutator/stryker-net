using Shouldly;
using Stryker.Abstractions;
using Stryker.Fsharp.Mutants;
using Stryker.Fsharp.Mutators;
using Stryker.Fsharp.UnitTest.Fixtures;

namespace Stryker.Fsharp.UnitTest;

[TestClass]
public class FsharpCollectionMutatorTests
{
    private const string Ns = "StrykerTest";

    private static List<Mutation> Mutations(string source)
    {
        var leaf = FsharpParse.Leaf(source);
        new FsharpMutantOrchestrator(Ns, [new CollectionFunctionMutator()]).Mutate(leaf);
        return leaf.Mutants.Select(m => m.Mutation).ToList();
    }

    [TestMethod]
    public void CollectionMutator_SeqHead_YieldsSeqLast()
    {
        var source = "module M\nlet f (l: int list) = l |> Seq.head\n";

        var mutation = Mutations(source).Single();

        mutation.OriginalText.ShouldBe("Seq.head");
        mutation.ReplacementText.ShouldBe("Seq.last");
        mutation.Type.ShouldBe(Mutator.Linq);
        mutation.Location.StartOffset.ShouldBe(source.IndexOf("Seq.head", StringComparison.Ordinal));
        new CollectionFunctionMutator().MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [TestMethod]
    [DataRow("min", "max")]
    [DataRow("max", "min")]
    [DataRow("minBy", "maxBy")]
    [DataRow("maxBy", "minBy")]
    [DataRow("head", "last")]
    [DataRow("last", "head")]
    [DataRow("tryHead", "tryLast")]
    [DataRow("tryLast", "tryHead")]
    [DataRow("sortBy", "sortByDescending")]
    [DataRow("sortByDescending", "sortBy")]
    [DataRow("sort", "sortDescending")]
    [DataRow("sortDescending", "sort")]
    [DataRow("take", "skip")]
    [DataRow("skip", "take")]
    [DataRow("takeWhile", "skipWhile")]
    [DataRow("skipWhile", "takeWhile")]
    [DataRow("exists", "forall")]
    [DataRow("forall", "exists")]
    public void CollectionMutator_Pair_YieldsExactReplacement(string original, string replacement)
    {
        foreach (var module in new[] { "Seq", "List", "Array" })
        {
            var mutation = Mutations($"module M\nlet f = {module}.{original}\n").Single();

            mutation.OriginalText.ShouldBe($"{module}.{original}");
            mutation.ReplacementText.ShouldBe($"{module}.{replacement}");
        }
    }

    [TestMethod]
    public void CollectionMutator_QualifiedModule_KeepsQualifier()
    {
        Mutations("module M\nlet f = Microsoft.FSharp.Collections.List.min\n").Single().ReplacementText
            .ShouldBe("Microsoft.FSharp.Collections.List.max");
    }

    [TestMethod]
    [DataRow("Seq.fold")]
    [DataRow("List.map")]
    [DataRow("Seq.sum")]
    [DataRow("Set.min")]
    [DataRow("Foo.head")]
    [DataRow("min")]
    public void CollectionMutator_UnmappedFunction_IsNotMutated(string ident)
    {
        Mutations($"module M\nlet f = {ident}\n").ShouldBeEmpty();
    }

    [TestMethod]
    public void CollectionMutator_ExtensionMethodStyle_IsNotMutated()
    {
        Mutations("module M\nopen System.Linq\nlet f (l: int list) = l.First()\n").ShouldBeEmpty();
    }

    [TestMethod]
    public void CollectionMutator_ReportCount_OnFixtureFile()
    {
        var source = "module M\nlet a (l: int list) = l |> List.min\nlet b (l: int list) = Seq.head l + Array.last [| 1 |]\nlet c (l: int list) = List.fold (+) 0 l\nlet d (l: int list) = l.Head\n";
        var leaf = FsharpParse.Leaf(source);

        new FsharpMutantOrchestrator(Ns, [new CollectionFunctionMutator()]).Mutate(leaf).ShouldBe(3);
        leaf.Mutants.Select(m => m.Mutation.ReplacementText).ShouldBe(["List.max", "Seq.last", "Array.head"], ignoreOrder: true);
    }

    [TestMethod]
    public void CollectionMutator_CompiledMutant_PipelineUsesMax()
    {
        using var project = new MutatedFsharpProject("module Lib\nlet pick (n: int) = [1; 3; 2] |> List.min\n");
        var mutant = project.Leaf.Mutants.Single(m => m.Mutation.Type == Mutator.Linq);
        var loaded = project.Load();

        loaded.Set("ActiveMutant", -1);
        loaded.Call("Lib", "pick", 1).ShouldBe(1);
        loaded.Set("ActiveMutant", mutant.Id);
        loaded.Call("Lib", "pick", 1).ShouldBe(3);
    }
}
