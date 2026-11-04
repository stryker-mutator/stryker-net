using FSharp.Compiler.Syntax;
using Stryker.Abstractions;
using Stryker.Fsharp.Mutants;

namespace Stryker.Fsharp.Mutators;

/// <summary>
/// Empties string literals (an empty one becomes "Stryker was here!") and replaces <c>String.Empty</c>.
/// Verbatim and triple-quoted literals are replaced whole by a plain literal.
/// </summary>
internal sealed class StringLiteralMutator : IFsharpMutator
{
    public MutationLevel MutationLevel => MutationLevel.Standard;

    private const string Filled = "\"Stryker was here!\"";

    public IEnumerable<Mutation> ApplyMutations(SynExpr expr, string sourceText)
    {
        switch (expr)
        {
            case SynExpr.Const { constant: SynConst.String str, range: var range }:
                yield return Create(range, sourceText, str.text.Length == 0 ? Filled : "\"\"");
                break;
            case SynExpr.LongIdent { longDotId: var id, range: var range } when IsStringEmpty(id):
                yield return Create(range, sourceText, Filled);
                break;
        }
    }

    private static bool IsStringEmpty(SynLongIdent id) =>
        id.LongIdent.Select(i => i.idText).ToArray() is ["String", "Empty"] or ["System", "String", "Empty"];

    private static Mutation Create(FSharp.Compiler.Text.Range range, string sourceText, string replacement)
    {
        var location = FsharpLocations.ToLocation(range, sourceText);
        return new Mutation
        {
            Location = location,
            OriginalText = sourceText[location.StartOffset..location.EndOffset],
            ReplacementText = replacement,
            Type = Mutator.String,
            DisplayName = "String mutation",
        };
    }
}
