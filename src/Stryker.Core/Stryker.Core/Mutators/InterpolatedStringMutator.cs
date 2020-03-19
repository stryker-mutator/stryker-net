using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class InterpolatedStringMutator: MutatorBase<InterpolatedStringExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Intermediate;

        public override IEnumerable<Mutation> ApplyMutations(InterpolatedStringExpressionSyntax node)
        {
            if (node.Contents.Any())
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = CreateEmptyInterpolatedString(),
                    DisplayName = @"String mutation",
                    Type = Mutator.String
                };
            }
        }

        private SyntaxNode CreateEmptyInterpolatedString()
        {
            var opening = SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken);
            var closing = SyntaxFactory.Token(SyntaxKind.InterpolatedStringEndToken);
            var emptyText = new SyntaxList<InterpolatedStringContentSyntax>
                {
                    SyntaxFactory.InterpolatedStringText()
                };
            return SyntaxFactory.InterpolatedStringExpression(opening, emptyText, closing);
        }
    }
}
