using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    /// <summary> Mutator Implementation for Math Mutations </summary>
    public class MathMutator : MutatorBase<ExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Advanced;

        /// <summary> Dictionary which maps original Math expressions to the target mutation </summary>
        private static Dictionary<MathExpression, IEnumerable<MathExpression>> KindsToMutate { get; }

        static MathMutator() => KindsToMutate = new()
        {
            [MathExpression.Acos] = new[] { MathExpression.Acosh, MathExpression.Asin, MathExpression.Atan },
            [MathExpression.Acosh] = new[] { MathExpression.Acos, MathExpression.Asinh, MathExpression.Atanh },
            [MathExpression.Asin] = new[] { MathExpression.Asinh, MathExpression.Acos, MathExpression.Atan },
            [MathExpression.Asinh] = new[] { MathExpression.Asin, MathExpression.Acosh, MathExpression.Atanh },
            [MathExpression.Atan] = new[] { MathExpression.Atanh, MathExpression.Acos, MathExpression.Asin },
            [MathExpression.Atanh] = new[] { MathExpression.Atan, MathExpression.Acosh, MathExpression.Asinh },
            [MathExpression.BitDecrement] = new[] { MathExpression.BitIncrement },
            [MathExpression.BitIncrement] = new[] { MathExpression.BitDecrement },
            [MathExpression.Ceiling] = new[] { MathExpression.Floor },
            [MathExpression.Cos] = new[] { MathExpression.Cosh, MathExpression.Sin, MathExpression.Tan },
            [MathExpression.Cosh] = new[] { MathExpression.Cos, MathExpression.Sinh, MathExpression.Tanh },
            [MathExpression.Exp] = new[] { MathExpression.Log },
            [MathExpression.Floor] = new[] { MathExpression.Ceiling },
            [MathExpression.Log] = new[] { MathExpression.Exp, MathExpression.Pow },
            [MathExpression.MaxMagnitude] = new[] { MathExpression.MinMagnitude },
            [MathExpression.MinMagnitude] = new[] { MathExpression.MaxMagnitude },
            [MathExpression.Pow] = new[] { MathExpression.Log },
            [MathExpression.ReciprocalEstimate] = new[] { MathExpression.ReciprocalSqrtEstimate },
            [MathExpression.ReciprocalSqrtEstimate] = new[] { MathExpression.ReciprocalEstimate, MathExpression.Sqrt },
            [MathExpression.Sin] = new[] { MathExpression.Sinh, MathExpression.Cos, MathExpression.Tan },
            [MathExpression.Sinh] = new[] { MathExpression.Sin, MathExpression.Cosh, MathExpression.Tanh },
            [MathExpression.Tan] = new[] { MathExpression.Tanh, MathExpression.Cos, MathExpression.Sin },
            [MathExpression.Tanh] = new[] { MathExpression.Tan, MathExpression.Cosh, MathExpression.Sinh }
        };

        /// <summary> Apply mutations to an <see cref="InvocationExpressionSyntax"/> </summary>
        public override IEnumerable<Mutation> ApplyMutations(ExpressionSyntax node)
        {
            var original = node;
            if (node.Parent is ConditionalAccessExpressionSyntax || node.Parent is MemberAccessExpressionSyntax)
            {
                yield break;
            }

            foreach (var mutation in FindMutableMethodCalls(node, original))
            {
                yield return mutation;
            }
        }

        private static IEnumerable<Mutation> FindMutableMethodCalls(ExpressionSyntax node, ExpressionSyntax original)
        {
            while (node is ConditionalAccessExpressionSyntax conditional)
            {
                foreach (var subMutants in FindMutableMethodCalls(conditional.Expression, original))
                {
                    yield return subMutants;
                }
                node = conditional.WhenNotNull;
            }

            for (; ; )
            {
                ExpressionSyntax next = null;
                if (node is not InvocationExpressionSyntax invocationExpression)
                {
                    yield break;
                }

                string memberName;
                SyntaxNode toReplace;
                switch (invocationExpression.Expression)
                {
                    case MemberAccessExpressionSyntax memberAccessExpression:
                        toReplace = memberAccessExpression.Name;
                        memberName = memberAccessExpression.Name.Identifier.ValueText;
                        next = memberAccessExpression.Expression;
                        break;
                    case MemberBindingExpressionSyntax memberBindingExpression:
                        toReplace = memberBindingExpression.Name;
                        memberName = memberBindingExpression.Name.Identifier.ValueText;
                        break;
                    default:
                        yield break;
                }

                if (Enum.TryParse(memberName, out MathExpression expression) &&
                    KindsToMutate.TryGetValue(expression, out var replacementExpressions))
                {
                    foreach (var replacementExpression in replacementExpressions)
                    {
                        yield return new Mutation
                        {
                            DisplayName =
                                $"Math method mutation ({memberName}() to {SyntaxFactory.IdentifierName(replacementExpression.ToString())}())",
                            OriginalNode = original,
                            ReplacementNode = original.ReplaceNode(toReplace,
                                SyntaxFactory.IdentifierName(replacementExpression.ToString())),
                            Type = Mutator.Math
                        };
                    }
                }

                node = next;
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
}
