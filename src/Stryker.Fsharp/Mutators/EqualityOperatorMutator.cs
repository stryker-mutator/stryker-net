using Stryker.Abstractions;

namespace Stryker.Fsharp.Mutators;

/// <summary>Swaps infix equality and comparison operators, with the same pairs, order and level as the C# binary expression mutator.</summary>
internal sealed class EqualityOperatorMutator() : InfixOperatorMutator(Mutator.Equality, "Equality mutation", MutationLevel.Basic, new()
{
    ["op_Equality"] = ("=", ["<>"]),
    ["op_Inequality"] = ("<>", ["="]),
    ["op_LessThan"] = ("<", [">", "<="]),
    ["op_LessThanOrEqual"] = ("<=", [">", "<"]),
    ["op_GreaterThan"] = (">", ["<", ">="]),
    ["op_GreaterThanOrEqual"] = (">=", ["<", ">"]),
});
