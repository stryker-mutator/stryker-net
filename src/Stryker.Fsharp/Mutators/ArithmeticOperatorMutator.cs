using Stryker.Abstractions;

namespace Stryker.Fsharp.Mutators;

/// <summary>Swaps infix arithmetic operators, with the same pairs and level as the C# binary expression mutator.</summary>
internal sealed class ArithmeticOperatorMutator() : InfixOperatorMutator(Mutator.Arithmetic, "Arithmetic mutation", MutationLevel.Basic, new()
{
    ["op_Addition"] = ("+", ["-"]),
    ["op_Subtraction"] = ("-", ["+"]),
    ["op_Multiply"] = ("*", ["/"]),
    ["op_Division"] = ("/", ["*"]),
    ["op_Modulus"] = ("%", ["*"]),
});
