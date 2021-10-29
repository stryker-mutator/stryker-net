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
            var kind = node.Kind();
            var root = node.SyntaxTree.GetRoot();

            if (!(root is ObjectCreationExpressionSyntax parsedRoot && IsRegexType(parsedRoot))
                && kind == SyntaxKind.StringLiteralExpression
                && !(node.Parent is ConstantPatternSyntax))
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

        private bool IsRegexType(ObjectCreationExpressionSyntax root)
        {
            var type = root.Type.ToString();
            return type == typeof(Regex).Name || type == typeof(Regex).FullName;
        }
    }
}
