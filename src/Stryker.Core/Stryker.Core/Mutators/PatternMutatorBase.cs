using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    /// <summary> Base Mutator implementation for expressions with patterns </summary>
    public abstract class PatternMutatorBase<T> : MutatorBase<T>, IMutator where T : ExpressionSyntax
    {
        public override MutationLevel MutationLevel => MutationLevel.Basic;

        /// <summary> Dictionary which maps original syntax kinds to target mutations </summary>
        /// <remarks>This could be a static field, but sonar does not like static fields for generic types, and the extra runtime cost is negligible.</remarks>
        private Dictionary<SyntaxKind, IEnumerable<SyntaxKind>> KindsToMutate { get; }

        protected PatternMutatorBase() => KindsToMutate = new()
        {
            [SyntaxKind.AndPattern] = new[] { SyntaxKind.OrPattern },
            [SyntaxKind.OrPattern] = new[] { SyntaxKind.AndPattern },
            [SyntaxKind.LessThanEqualsToken] = new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanToken },
            [SyntaxKind.GreaterThanEqualsToken] = new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanToken },
            [SyntaxKind.LessThanToken] = new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanEqualsToken },
            [SyntaxKind.GreaterThanToken] = new[] { SyntaxKind.LessThanToken, SyntaxKind.GreaterThanEqualsToken }
        };

        /// <summary> Apply mutations to a <see cref="PatternSyntax"/></summary>
        protected IEnumerable<Mutation> ApplyMutations(PatternSyntax node) => node switch
        {
            BinaryPatternSyntax binaryPattern => ApplyMutations(binaryPattern),
            RelationalPatternSyntax relationalPattern => ApplyMutations(relationalPattern),
            _ => Enumerable.Empty<Mutation>()
        };

        private IEnumerable<Mutation> ApplyMutations(BinaryPatternSyntax node)
        {
            if (KindsToMutate.TryGetValue(node.Kind(), out var mutations))
            {
                foreach (var mutation in mutations)
                {
                    var replacementNode = SyntaxFactory.BinaryPattern(mutation, node.Left, node.Right);
                    replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken));

                    yield return new()
                    {
                        OriginalNode = node,
                        ReplacementNode = replacementNode,
                        DisplayName = $"Logical mutation",
                        Type = Mutator.Logical
                    };
                }
            }
        }

        private IEnumerable<Mutation> ApplyMutations(RelationalPatternSyntax node)
        {
            if (KindsToMutate.TryGetValue(node.OperatorToken.Kind(), out var mutations))
            {
                foreach (var mutation in mutations)
                {
                    var replacementNode = SyntaxFactory.RelationalPattern(SyntaxFactory.Token(mutation), node.Expression);
                    replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken));

                    yield return new()
                    {
                        OriginalNode = node,
                        ReplacementNode = replacementNode,
                        DisplayName = $"Equality mutation",
                        Type = Mutator.Equality
                    };
                }
            }
        }
    }
}
