using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutators;

/// <summary> Mutator Implementation for Math Mutations </summary>
public class MathMutator : MutatorBase<InvocationExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Advanced;

    /// <summary> Dictionary which maps original Math expressions to the target mutation </summary>
    private static Dictionary<MathExpression, IEnumerable<MathExpression>> KindsToMutate { get; }

    // Known Math method names used for preliminary filtering to improve performance.
    private static readonly HashSet<string> KnownMathMethodNames = new HashSet<string>(StringComparer.Ordinal)
    {
        "Acos", "Acosh", "Asin", "Asinh", "Atan", "Atanh",
        "BitDecrement", "BitIncrement", "Ceiling", "Cos", "Cosh",
        "Exp", "Floor", "Log", "MaxMagnitude", "MinMagnitude",
        "Pow", "ReciprocalEstimate", "ReciprocalSqrtEstimate",
        "Sin", "Sinh", "Sqrt", "Tan", "Tanh"
    };

    static MathMutator() => KindsToMutate = new()
    {
        [MathExpression.Acos] = [MathExpression.Acosh, MathExpression.Asin, MathExpression.Atan],
        [MathExpression.Acosh] = [MathExpression.Acos, MathExpression.Asinh, MathExpression.Atanh],
        [MathExpression.Asin] = [MathExpression.Asinh, MathExpression.Acos, MathExpression.Atan],
        [MathExpression.Asinh] = [MathExpression.Asin, MathExpression.Acosh, MathExpression.Atanh],
        [MathExpression.Atan] = [MathExpression.Atanh, MathExpression.Acos, MathExpression.Asin],
        [MathExpression.Atanh] = [MathExpression.Atan, MathExpression.Acosh, MathExpression.Asinh],
        [MathExpression.BitDecrement] = [MathExpression.BitIncrement],
        [MathExpression.BitIncrement] = [MathExpression.BitDecrement],
        [MathExpression.Ceiling] = [MathExpression.Floor],
        [MathExpression.Cos] = [MathExpression.Cosh, MathExpression.Sin, MathExpression.Tan],
        [MathExpression.Cosh] = [MathExpression.Cos, MathExpression.Sinh, MathExpression.Tanh],
        [MathExpression.Exp] = [MathExpression.Log],
        [MathExpression.Floor] = [MathExpression.Ceiling],
        [MathExpression.Log] = [MathExpression.Exp, MathExpression.Pow],
        [MathExpression.MaxMagnitude] = [MathExpression.MinMagnitude],
        [MathExpression.MinMagnitude] = [MathExpression.MaxMagnitude],
        [MathExpression.Pow] = [MathExpression.Log],
        [MathExpression.ReciprocalEstimate] = [MathExpression.ReciprocalSqrtEstimate],
        [MathExpression.ReciprocalSqrtEstimate] = [MathExpression.ReciprocalEstimate, MathExpression.Sqrt],
        [MathExpression.Sin] = [MathExpression.Sinh, MathExpression.Cos, MathExpression.Tan],
        [MathExpression.Sinh] = [MathExpression.Sin, MathExpression.Cosh, MathExpression.Tanh],
        [MathExpression.Tan] = [MathExpression.Tanh, MathExpression.Cos, MathExpression.Sin],
        [MathExpression.Tanh] = [MathExpression.Tan, MathExpression.Cosh, MathExpression.Sinh]
    };

    /// <summary> Apply mutations to an <see cref="InvocationExpressionSyntax"/> </summary>
    public override IEnumerable<Mutation> ApplyMutations(InvocationExpressionSyntax node, SemanticModel semanticModel) => node.Expression switch
    {
        MemberAccessExpressionSyntax memberAccess => ApplyMutationsToMemberCall(node, memberAccess, semanticModel),
        IdentifierNameSyntax methodName => ApplyMutationsToDirectCall(node, methodName, semanticModel),
        _ => []
    };

    private static IEnumerable<Mutation> ApplyMutationsToMemberCall(InvocationExpressionSyntax node, MemberAccessExpressionSyntax memberAccessExpressionSyntax, SemanticModel semanticModel)
    {
        var methodNameText = memberAccessExpressionSyntax.Name.Identifier.Text;
        if (!KnownMathMethodNames.Contains(methodNameText))
        {
            yield break;
        }

        var symbol = semanticModel.GetSymbolInfo(memberAccessExpressionSyntax.Expression).Symbol;
        if (symbol?.ContainingType?.ToString() != "System.Math")
        {
            yield break;
        }

        foreach (var mutation in ApplyMutationsToMethod(node, memberAccessExpressionSyntax.Name))
        {
            yield return mutation;
        }
    }

    private static IEnumerable<Mutation> ApplyMutationsToDirectCall(InvocationExpressionSyntax node, IdentifierNameSyntax methodName, SemanticModel semanticModel)
    {
        var methodNameText = methodName.Identifier.Text;
        if (!KnownMathMethodNames.Contains(methodNameText))
        {
            yield break;
        }

        var symbol = semanticModel.GetSymbolInfo(methodName).Symbol;
        if (symbol?.ContainingType?.ToString() != "System.Math")
        {
            yield break;
        }

        foreach (var mutation in ApplyMutationsToMethod(node, methodName))
        {
            yield return mutation;
        }
    }

    private static IEnumerable<Mutation> ApplyMutationsToMethod(InvocationExpressionSyntax original, SimpleNameSyntax method)
    {
        if (Enum.TryParse(method.Identifier.ValueText, out MathExpression expression) &&
            KindsToMutate.TryGetValue(expression, out var replacementExpressions))
        {
            foreach (var replacementExpression in replacementExpressions)
            {
                yield return new Mutation
                {
                    DisplayName =
                        $"Math method mutation ({method.Identifier.ValueText}() to {SyntaxFactory.IdentifierName(replacementExpression.ToString())}())",
                    OriginalNode = original,
                    ReplacementNode = original.ReplaceNode(method,
                        SyntaxFactory.IdentifierName(replacementExpression.ToString())).WithCleanTrivia(),
                    Type = Mutator.Math
                };
            }
        }
    }
}

/// <summary> Enumeration for the different kinds of Math expressions </summary>
public enum MathExpression
{
    Acos,
    Acosh,
    Asin,
    Asinh,
    Atan,
    Atanh,
    BitDecrement,
    BitIncrement,
    Ceiling,
    Cos,
    Cosh,
    Exp,
    Floor,
    Log,
    MaxMagnitude,
    MinMagnitude,
    Pow,
    ReciprocalEstimate,
    ReciprocalSqrtEstimate,
    Sin,
    Sinh,
    Sqrt,
    Tan,
    Tanh
}
