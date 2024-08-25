using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators;

public class IntegerNullificationMutator : MutatorBase<LiteralExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    public override IEnumerable<Mutation> ApplyMutations(LiteralExpressionSyntax node, SemanticModel semanticModel)
    {
        if (IsNumberLiteral(node) && node.Token.Value is int value && value != 0)
        {
            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)),
                DisplayName = "Integer nullification mutation",
                Type = Mutator.Number
            };
        }
    }

    private static bool IsNumberLiteral(LiteralExpressionSyntax node)
    {
        var kind = node.Kind();
        return kind == SyntaxKind.NumericLiteralExpression && node.Parent is EqualsValueClauseSyntax;
    }
}
