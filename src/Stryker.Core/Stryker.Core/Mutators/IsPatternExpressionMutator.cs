using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    /// <summary> Mutator implementation for is expression</summary>
    public class IsPatternExpressionMutator : PatternMutatorBase<IsPatternExpressionSyntax>
    {
        /// <summary>
        /// Apply mutations to all <see cref="PatternSyntax"/> inside an <see cref="IsPatternExpressionSyntax"/>.
        /// Apply mutations to the root pattern.
        /// </summary>
        public override IEnumerable<Mutation> ApplyMutations(IsPatternExpressionSyntax node)
        {
            yield return ReverseRootPattern(node);

            var descendantMutations = node
                .DescendantNodes()
                .OfType<PatternSyntax>()
                .SelectMany(x => ApplyMutations(x));

            foreach (var descendantMutation in descendantMutations)
            {
                yield return descendantMutation;
            }
        }

        private Mutation ReverseRootPattern(IsPatternExpressionSyntax node) => node.Pattern switch
        {
            UnaryPatternSyntax notPattern => new Mutation
            {
                OriginalNode = notPattern,
                ReplacementNode = notPattern.Pattern,
                Type = Mutator.Equality,
                DisplayName = "Equality mutation"
            },
            _ => new Mutation
            {
                OriginalNode = node.Pattern,
                ReplacementNode = SyntaxFactory.UnaryPattern(node.Pattern.WithLeadingTrivia(SyntaxFactory.Space)),
                Type = Mutator.Equality,
                DisplayName = "Equality mutation"
            }
        };
    }
}
