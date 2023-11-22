using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators;

public class InterpolatedStringMutator : MutatorBase<InterpolatedStringExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    public override IEnumerable<Mutation> ApplyMutations(InterpolatedStringExpressionSyntax node, SemanticModel semanticModel)
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
