using FSharp.Compiler.Syntax;
using Stryker.Abstractions;
using Stryker.Fsharp.Mutants;

namespace Stryker.Fsharp.Mutators;

/// <summary>
/// F# analogue of the C# LinqMutator: swaps a <c>Seq</c>/<c>List</c>/<c>Array</c> module function for its counterpart
/// (<c>l |> Seq.max</c> for <c>l |> Seq.min</c>). Only the function name changes; the qualifier is kept.
/// Extension-method LINQ on F# values (<c>l.First()</c>) is out of scope and not matched.
/// </summary>
internal sealed class CollectionFunctionMutator : IFsharpMutator
{
    public MutationLevel MutationLevel => MutationLevel.Standard;

    private static readonly HashSet<string> Modules = ["Seq", "List", "Array"];

    // Adopted pairs follow the C# linq table where a same-signature module function exists on all three modules.
    // Skipped: First/FirstOrDefault, Single*, Count/Sum/Average (no same-typed counterpart; Sum/Average need member
    // constraints), Reverse/AsEnumerable, Union/Intersect/Concat/Except/*By (set or sequence-combining shapes differ),
    // Append/Prepend (take a sequence, not an element). C# Last->First becomes the symmetric head<->last.
    private static readonly Dictionary<string, string> Pairs = BuildPairs(
        ("min", "max"),
        ("minBy", "maxBy"),
        ("head", "last"),
        ("tryHead", "tryLast"),
        ("sortBy", "sortByDescending"),
        ("sort", "sortDescending"),
        ("take", "skip"),
        ("takeWhile", "skipWhile"),
        ("exists", "forall"));

    private static Dictionary<string, string> BuildPairs(params (string, string)[] pairs) =>
        pairs.SelectMany(p => new[] { p, (p.Item2, p.Item1) }).ToDictionary(p => p.Item1, p => p.Item2);

    public IEnumerable<Mutation> ApplyMutations(SynExpr expr, string sourceText)
    {
        if (expr is not SynExpr.LongIdent { longDotId: var id, range: var range })
        {
            yield break;
        }

        var idents = id.LongIdent;
        if (idents.Length < 2 || !Modules.Contains(idents[^2].idText) || !Pairs.TryGetValue(idents[^1].idText, out var replacement))
        {
            yield break;
        }

        var name = idents[^1].idText;
        var location = FsharpLocations.ToLocation(range, sourceText);
        var original = sourceText[location.StartOffset..location.EndOffset];
        var nameStart = original.LastIndexOf(name, StringComparison.Ordinal);
        yield return new Mutation
        {
            Location = location,
            OriginalText = original,
            ReplacementText = original[..nameStart] + replacement + original[(nameStart + name.Length)..],
            Type = Mutator.Linq,
            DisplayName = $"Linq method mutation ({name}() to {replacement}())",
        };
    }
}
