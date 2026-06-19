using System.Collections.Generic;
using System.Linq;
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
            // An empty initializer would fail to compile with CS9035 when the type has
            // required members, so we preserve them by assigning `default!` to each.
            var requiredMembers = GetRequiredMemberNames(node, semanticModel);
            var replacementInitializer = requiredMembers.Count == 0
                ? SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression)
                : SyntaxFactory.InitializerExpression(
                    SyntaxKind.ObjectInitializerExpression,
                    SyntaxFactory.SeparatedList<ExpressionSyntax>(
                        requiredMembers.Select(name =>
                            SyntaxFactory.AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                SyntaxFactory.IdentifierName(name),
                                SyntaxFactory.PostfixUnaryExpression(
                                    SyntaxKind.SuppressNullableWarningExpression,
                                    SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression))))));

            yield return new Mutation()
            {
                OriginalNode = node,
                ReplacementNode = node.ReplaceNode(node.Initializer, replacementInitializer).WithCleanTrivia(),
                DisplayName = "Object initializer mutation",
                Type = Mutator.Initializer,
            };
        }
    }

    private static IReadOnlyList<string> GetRequiredMemberNames(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
    {
        if (semanticModel is null)
        {
            return [];
        }

        if (semanticModel.GetTypeInfo(node).Type is not INamedTypeSymbol typeSymbol)
        {
            return [];
        }

        var names = new List<string>();
        var seen = new HashSet<string>();
        for (var current = typeSymbol; current is not null; current = current.BaseType)
        {
            foreach (var member in current.GetMembers())
            {
                var isRequired = (member is IPropertySymbol prop && prop.IsRequired) ||
                                 (member is IFieldSymbol field && field.IsRequired);
                if (isRequired && seen.Add(member.Name))
                {
                    names.Add(member.Name);
                }
            }
        }

        return names;
    }
}
