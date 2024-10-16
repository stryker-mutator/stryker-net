using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Stryker.Core.Mutators;

public sealed class CollectionExpressionMutator : MutatorBase<CollectionExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Advanced;

    private static ExpressionElementSyntax DefaultElementSyntax =>
        ExpressionElement(LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword)));

    public override IEnumerable<Mutation> ApplyMutations(CollectionExpressionSyntax node, SemanticModel semanticModel)
    {
        if (node.Elements.Count > 0)
        {
            var type = semanticModel?.GetOperation(node)?.Type;

            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode =
                    type is not null
                        ? CastExpression(ParseTypeName(type.ToMinimalDisplayString(semanticModel, node.SpanStart)),
                                         node.WithElements([]))
                        : node.WithElements([]),
                DisplayName = "Collection expression mutation",
                Type        = Mutator.CollectionExpression
            };
        }
        else
        {
            yield return new Mutation
            {
                OriginalNode    = node,
                ReplacementNode = node.AddElements(DefaultElementSyntax),
                DisplayName     = "Collection expression mutation",
                Type            = Mutator.CollectionExpression
            };

            yield return new Mutation
            {
                OriginalNode    = node,
                ReplacementNode = node.AddElements(DefaultElementSyntax, DefaultElementSyntax),
                DisplayName     = "Collection expression mutation",
                Type            = Mutator.CollectionExpression
            };
        }
    }
}
