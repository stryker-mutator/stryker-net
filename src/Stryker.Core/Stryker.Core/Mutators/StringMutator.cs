using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stryker.Core.Mutators
{
    public class StringMutator : MutatorBase<LiteralExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(LiteralExpressionSyntax node)
        {
            // Get objectCreationSyntax to check if it contains a regex type.
            var root = node.Parent?.Parent?.Parent;

            if (!IsRegexType(root) && IsStringLiteral(node))
            {
                var currentValue = (string)node.Token.Value;
                var replacementValue = currentValue == "" ? "Stryker was here!" : "";
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(replacementValue)),
                    DisplayName = @"String mutation",
                    Type = Mutator.String
                };
            }
        }

        private bool IsStringLiteral(LiteralExpressionSyntax node)
        {
            var kind = node.Kind();
            return kind == SyntaxKind.StringLiteralExpression
                && !(node.Parent is ConstantPatternSyntax);
        }

        private bool IsRegexType(SyntaxNode root)
        {
            if (root is ObjectCreationExpressionSyntax parsedRoot)
            {
                var type = parsedRoot.Type.ToString();
                return type == typeof(Regex).Name || type == typeof(Regex).FullName;
            }

            return false;
        }
    }
}
