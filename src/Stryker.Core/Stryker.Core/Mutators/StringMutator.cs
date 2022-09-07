using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutators
{
    public class StringMutator: MutatorBase<LiteralExpressionSyntax>
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(LiteralExpressionSyntax node)
        {
            // Get objectCreationSyntax to check if it contains a regex type.
            var root = node.Parent?.Parent?.Parent;

            if (!IsRegexType(root) && node.IsStringLiteral() && node.Parent is not ConstantPatternSyntax)
            {
                var currentValue = (string)node.Token.Value;
                var replacementValue = currentValue == "" ? "Stryker was here!" : "";
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(replacementValue)),
                    DisplayName = "String mutation",
                    Type = Mutator.String
                };
            }
        }

        private bool IsRegexType(SyntaxNode root)
        {
            if (root is not ObjectCreationExpressionSyntax parsedRoot)
            {
                return false;
            }
            var type = parsedRoot.Type.ToString();
            return type == nameof(Regex) || type == typeof(Regex).FullName;

        }
    }
}
