using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.RegexMutators;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stryker.Core.Mutators
{
    public class RegexMutator : MutatorBase<ObjectCreationExpressionSyntax>, IMutator
    {
        private ILogger Logger { get; }

        public RegexMutator()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<RegexMutator>();
        }

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
                    foreach (RegexMutation regexMutation in replacementValues)
                    {
                        try
                        {
                            _ = new Regex(regexMutation.Pattern);
                        }
                        catch (ArgumentException exception)
                        {
                            Logger.LogDebug($"RegexMutator created mutation {currentValue} -> {regexMutation.Pattern} which is an invalid regular expression:\n{exception.Message}");
                            continue;
                        }

                        yield return new Mutation()
                        {
                            OriginalNode = patternExpression,
                            ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(regexMutation.Pattern)),
                            DisplayName = regexMutation.DisplayName,
                            Type = Mutator.Regex,
                            Description = regexMutation.Description
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