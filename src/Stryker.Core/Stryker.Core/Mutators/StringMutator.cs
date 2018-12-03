using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class StringMutator: Mutator<LiteralExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(LiteralExpressionSyntax node)
        {
            var kind = node.Kind();
            if (kind == SyntaxKind.StringLiteralExpression)
            {
                var currentValue = (string) node.Token.Value;
                var replacementValue = currentValue == "" ? "Stryker was here!" : "";
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(replacementValue)),
                    DisplayName = @"String mutation",
                    Type = MutatorType.String
                };
            }
        }
    }
}
