using System.Collections.Generic;
using FSharp.Compiler.Syntax;
using Stryker.Abstractions;
using Stryker.Fsharp.Mutants;

namespace Stryker.Fsharp.Mutators;

/// <summary>
/// Swaps infix operators looked up by their FCS identifier (<c>op_Addition</c>...). The mutation covers the whole
/// binary expression (as in C#); the replacement is its text with the operator token swapped.
/// </summary>
internal abstract class InfixOperatorMutator : IFsharpMutator
{
    private readonly Dictionary<string, (string Symbol, string[] Replacements)> _operators;

    protected InfixOperatorMutator(Mutator type, string displayName, MutationLevel level, Dictionary<string, (string Symbol, string[] Replacements)> operators)
    {
        Type = type;
        DisplayName = displayName;
        MutationLevel = level;
        _operators = operators;
    }

    public MutationLevel MutationLevel { get; }

    private Mutator Type { get; }

    private string DisplayName { get; }

    public IEnumerable<Mutation> ApplyMutations(SynExpr expr, string sourceText)
    {
        // a + b parses as App(App(op_Addition [infix], a), b); sections and unary minus have no infix application
        if (expr is not SynExpr.App { funcExpr: SynExpr.App { isInfix: true, funcExpr: SynExpr.LongIdent { longDotId: var name } } })
        {
            yield break;
        }

        if (name.LongIdent.Length != 1 || !_operators.TryGetValue(name.LongIdent.Head.idText, out var pair))
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

        foreach (var replacement in pair.Replacements)
        {
            yield return new Mutation
            {
                Location = location,
                OriginalText = original,
                ReplacementText = original[..operatorStart] + replacement + original[(operatorStart + operatorLength)..],
                Type = Type,
                DisplayName = $"{DisplayName} ({pair.Symbol} to {replacement})",
            };
        }
    }
}
