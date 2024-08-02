using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Configuration.Mutants;
using System.Collections.Generic;

namespace Stryker.Configuration.Mutators;

public class BooleanMutator : MutatorBase<LiteralExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;
    public override IEnumerable<Mutation> ApplyMutations(LiteralExpressionSyntax node, SemanticModel semanticModel)
    {
        if (node.Kind() == SyntaxKind.TrueLiteralExpression)
        {
            yield return new Mutation()
            {
                OriginalNode = node,
                ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression),
                DisplayName = "Boolean mutation",
                Type = Mutator.Boolean
            };
        }
        else if (node.Kind() == SyntaxKind.FalseLiteralExpression)
        {
            yield return new Mutation()
            {
                OriginalNode = node,
                ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression),
                DisplayName = "Boolean mutation",
                Type = Mutator.Boolean
            };
        }
    }
}
