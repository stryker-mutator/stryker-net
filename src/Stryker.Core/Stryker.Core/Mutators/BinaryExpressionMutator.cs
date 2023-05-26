using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutators
{
    public class BinaryExpressionMutator : MutatorBase<BinaryExpressionSyntax>, IMutator
    {
        private readonly struct MutationData
        {
            public readonly Mutator Mutator;
            public readonly IEnumerable<SyntaxKind> KindsToMutate;

            public MutationData(Mutator mutator, params SyntaxKind[] kindsToMutate)
            {
                Mutator = mutator;
                KindsToMutate = kindsToMutate;
            }

        }

        private static readonly Dictionary<SyntaxKind, MutationData> _kindsToMutate = new Dictionary<SyntaxKind, MutationData>()
        {
            { SyntaxKind.SubtractExpression, new MutationData(Mutator.Arithmetic, SyntaxKind.AddExpression) },
            { SyntaxKind.AddExpression, new MutationData(Mutator.Arithmetic, SyntaxKind.SubtractExpression) },
            { SyntaxKind.MultiplyExpression, new MutationData(Mutator.Arithmetic, SyntaxKind.DivideExpression) },
            { SyntaxKind.DivideExpression, new MutationData(Mutator.Arithmetic, SyntaxKind.MultiplyExpression) },
            { SyntaxKind.ModuloExpression, new MutationData(Mutator.Arithmetic, SyntaxKind.MultiplyExpression) },
            { SyntaxKind.GreaterThanExpression, new MutationData(Mutator.Equality, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanOrEqualExpression) },
            { SyntaxKind.LessThanExpression, new MutationData(Mutator.Equality, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanOrEqualExpression) },
            { SyntaxKind.GreaterThanOrEqualExpression, new MutationData(Mutator.Equality, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanExpression) },
            { SyntaxKind.LessThanOrEqualExpression, new MutationData(Mutator.Equality, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression) },
            { SyntaxKind.EqualsExpression, new MutationData(Mutator.Equality, SyntaxKind.NotEqualsExpression) },
            { SyntaxKind.NotEqualsExpression, new MutationData(Mutator.Equality, SyntaxKind.EqualsExpression) },
            { SyntaxKind.LogicalAndExpression, new MutationData(Mutator.Logical, SyntaxKind.LogicalOrExpression) },
            { SyntaxKind.LogicalOrExpression, new MutationData(Mutator.Logical, SyntaxKind.LogicalAndExpression) },
            { SyntaxKind.LeftShiftExpression, new MutationData(Mutator.Bitwise, SyntaxKind.RightShiftExpression) },
            { SyntaxKind.RightShiftExpression, new MutationData(Mutator.Bitwise, SyntaxKind.LeftShiftExpression) },
            { SyntaxKind.BitwiseOrExpression, new MutationData(Mutator.Bitwise, SyntaxKind.BitwiseAndExpression) },
            { SyntaxKind.BitwiseAndExpression, new MutationData(Mutator.Bitwise, SyntaxKind.BitwiseOrExpression) },
        };

        public override MutationLevel MutationLevel => MutationLevel.Basic;

        public override IEnumerable<Mutation> ApplyMutations(BinaryExpressionSyntax node)
        {
            // skip string additions
            if (node.Kind() == SyntaxKind.AddExpression && (node.Left.IsAStringExpression() || node.Right.IsAStringExpression()))
            {
                yield break;
            }

            if (_kindsToMutate.TryGetValue(node.Kind(), out var mutationData))
            {
                foreach (var mutationKind in mutationData.KindsToMutate)
                {
                    var replacementNode = SyntaxFactory.BinaryExpression(mutationKind, node.Left, node.Right);
                    // make sure the trivia stays in place for displaying
                    replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken));
                    yield return new Mutation()
                    {
                        OriginalNode = node,
                        ReplacementNode = replacementNode,
                        DisplayName = $"{mutationData.Mutator} mutation",
                        Type = mutationData.Mutator
                    };
                }
            }
            else if (node.Kind() == SyntaxKind.ExclusiveOrExpression)
            {
                // Place both a logical and an integral mutation. Only one will compile, the other will be removed. See: https://github.com/stryker-mutator/stryker-net/issues/664
                yield return GetLogicalMutation(node);
                yield return GetIntegralMutation(node);
            }
        }

        private static Mutation GetLogicalMutation(BinaryExpressionSyntax node)
        {
            var replacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, node.Left, node.Right);
            replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken));

            return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = replacementNode,
                DisplayName = "Logical mutation",
                Type = Mutator.Logical
            };
        }

        private static Mutation GetIntegralMutation(ExpressionSyntax node)
        {
            var replacementNode = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.BitwiseNotExpression, SyntaxFactory.ParenthesizedExpression(node));

            return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = replacementNode,
                DisplayName = "Bitwise mutation",
                Type = Mutator.Bitwise
            };
        }
    }
}
