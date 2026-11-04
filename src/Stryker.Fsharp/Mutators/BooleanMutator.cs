using FSharp.Compiler.Syntax;
using Stryker.Abstractions;
using Stryker.Fsharp.Mutants;

namespace Stryker.Fsharp.Mutators;

/// <summary>Flips boolean literals.</summary>
internal sealed class BooleanMutator : IFsharpMutator
{
    public MutationLevel MutationLevel => MutationLevel.Standard;

    public IEnumerable<Mutation> ApplyMutations(SynExpr expr, string sourceText)
    {
        if (expr is not SynExpr.Const { constant: SynConst.Bool value, range: var range })
        {
            yield break;
        }

        var location = FsharpLocations.ToLocation(range, sourceText);
        var original = sourceText[location.StartOffset..location.EndOffset];
        yield return new Mutation
        {
            Location = location,
            OriginalText = original,
            ReplacementText = value.Item ? "false" : "true",
            Type = Mutator.Boolean,
            DisplayName = $"Boolean mutation ({original} to {(value.Item ? "false" : "true")})",
        };
    }
}
