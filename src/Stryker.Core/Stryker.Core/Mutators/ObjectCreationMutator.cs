using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutators;

public class ObjectCreationMutator : MutatorBase<ObjectCreationExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    public override IEnumerable<Mutation> ApplyMutations(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
    {
        if (node.Initializer?.Kind() == SyntaxKind.CollectionInitializerExpression && node.Initializer.Expressions.Count > 0)
        {
            yield return new Mutation()
            {
                OriginalNode = node,
                ReplacementNode = node.ReplaceNode(node.Initializer, SyntaxFactory.InitializerExpression(SyntaxKind.CollectionInitializerExpression)).WithCleanTrivia(),
                DisplayName = "Collection initializer mutation",
                Type = Mutator.Initializer
            };
        }
        if (node.Initializer?.Kind() == SyntaxKind.ObjectInitializerExpression && node.Initializer.Expressions.Count > 0)
        {
            // Skip when the target type has any `required` members — an empty
            // initializer would fail to compile with CS8852.
            if (HasRequiredMembers(node, semanticModel))
            {
                yield break;
            }

            yield return new Mutation()
            {
                OriginalNode = node,
                ReplacementNode = node.ReplaceNode(node.Initializer, SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression)).WithCleanTrivia(),
                DisplayName = "Object initializer mutation",
                Type = Mutator.Initializer,
            };
        }
    }

    private static bool HasRequiredMembers(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
    {
        if (semanticModel is null)
        {
            return false;
        }

        var typeSymbol = semanticModel.GetTypeInfo(node).Type as INamedTypeSymbol;
        if (typeSymbol is null)
        {
            return false;
        }

        for (var current = typeSymbol; current is not null; current = current.BaseType)
        {
            foreach (var member in current.GetMembers())
            {
                if ((member is IPropertySymbol prop && prop.IsRequired) ||
                    (member is IFieldSymbol field && field.IsRequired))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
