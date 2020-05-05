using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using Stryker.RegexMutators;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stryker.Core.Mutators
{
    public class RegexMutator : MutatorBase<ObjectCreationExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(ObjectCreationExpressionSyntax node)
        {
            string name = GetTypeName(node);
            if (name == typeof(Regex).Name || name == typeof(Regex).FullName)
            {
                var patternExpression = node.ArgumentList.Arguments.FirstOrDefault()?.Expression;
                if (patternExpression?.Kind() == SyntaxKind.StringLiteralExpression)
                {
                    var currentValue = ((LiteralExpressionSyntax)patternExpression).Token.ValueText;
                    var regexMutantOrchestrator = new RegexMutantOrchestrator(currentValue);
                    var replacementValues = regexMutantOrchestrator.Mutate();
                    foreach (string replacementValue in replacementValues)
                    {
                        yield return new Mutation()
                        {
                            OriginalNode = patternExpression,
                            ReplacementNode = patternExpression.ReplaceNode(patternExpression, SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(replacementValue))),
                            DisplayName = "Regex mutation",
                            Type = Mutator.Regex
                        };
                    }
                }
            }
        }

        private string GetTypeName(ObjectCreationExpressionSyntax node)
        {
            if (node.Type.Kind() == SyntaxKind.IdentifierName)
            {
                return ((IdentifierNameSyntax)node.Type).Identifier.ValueText;
            }

            else if (node.Type.Kind() == SyntaxKind.QualifiedName)
            {
                var qualifiedName = (QualifiedNameSyntax)node.Type;
                var name = qualifiedName.Right.Identifier.ValueText;
                while (qualifiedName.Left.Kind() == SyntaxKind.QualifiedName)
                {
                    qualifiedName = (QualifiedNameSyntax)qualifiedName.Left;
                    name = $"{qualifiedName.Right.Identifier.ValueText}.{name}";
                }
                return $"{((IdentifierNameSyntax)qualifiedName.Left).Identifier.ValueText}.{name}";
            }

            return null;
        }
    }
}