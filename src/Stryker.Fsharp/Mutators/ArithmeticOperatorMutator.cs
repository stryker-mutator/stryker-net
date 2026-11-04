using FSharp.Compiler.Syntax;
using Stryker.Abstractions;
using Stryker.Fsharp.Mutants;

namespace Stryker.Fsharp.Mutators;

/// <summary>
/// Swaps infix arithmetic operators, with the same pairs as the C# binary expression mutator.
/// The mutation covers the whole binary expression (as in C#); the replacement is its text with the operator token swapped.
/// </summary>
internal sealed class ArithmeticOperatorMutator : IFsharpMutator
{
    private static readonly Dictionary<string, (string Symbol, string Replacement)> Operators = new()
    {
        ["op_Addition"] = ("+", "-"),
        ["op_Subtraction"] = ("-", "+"),
        ["op_Multiply"] = ("*", "/"),
        ["op_Division"] = ("/", "*"),
        ["op_Modulus"] = ("%", "*"),
    };

    public MutationLevel MutationLevel => MutationLevel.Basic;

    public IEnumerable<Mutation> ApplyMutations(SynExpr expr, string sourceText)
    {
        // a + b parses as App(App(op_Addition [infix], a), b); sections and unary minus have no infix application
        if (expr is not SynExpr.App { funcExpr: SynExpr.App { isInfix: true, funcExpr: SynExpr.LongIdent { longDotId: var name } } })
        {
            yield break;
        }

        if (name.LongIdent.Length != 1 || !Operators.TryGetValue(name.LongIdent.Head.idText, out var pair))
        {
            yield break;
        }

        var location = FsharpLocations.ToLocation(expr.Range, sourceText);
        var operatorLocation = FsharpLocations.ToLocation(name.LongIdent.Head.idRange, sourceText);
        var operatorStart = operatorLocation.StartOffset - location.StartOffset;
        var operatorLength = operatorLocation.EndOffset - operatorLocation.StartOffset;
        var original = sourceText[location.StartOffset..location.EndOffset];
        if (operatorLength != pair.Symbol.Length || original.Substring(operatorStart, operatorLength) != pair.Symbol)
        {
            yield break;
        }

        yield return new Mutation
        {
            Location = location,
            OriginalText = original,
            ReplacementText = original[..operatorStart] + pair.Replacement + original[(operatorStart + operatorLength)..],
            Type = Mutator.Arithmetic,
            DisplayName = $"Arithmetic mutation ({pair.Symbol} to {pair.Replacement})",
        };
    }
}
