using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.RegexMutators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stryker.Core.Mutators;

public class RegexMutator : MutatorBase<ObjectCreationExpressionSyntax>
{
    private const string PatternArgumentName = "pattern";
    private ILogger Logger { get; }

    public override MutationLevel MutationLevel => MutationLevel.Advanced;

    public RegexMutator()
    {
        Logger = ApplicationLogging.LoggerFactory.CreateLogger<RegexMutator>();
    }

    public override IEnumerable<Mutation> ApplyMutations(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
    {
        var name = node.Type.ToString();
        if (name == nameof(Regex) || name == typeof(Regex).FullName)
        {
            var arguments = node.ArgumentList.Arguments;
            var namedArgument = arguments.FirstOrDefault(argument => argument.NameColon?.Name.Identifier.ValueText == PatternArgumentName);
            var patternArgument = namedArgument ?? node.ArgumentList.Arguments.FirstOrDefault();
            var patternExpression = patternArgument?.Expression;

            if (patternExpression?.Kind() == SyntaxKind.StringLiteralExpression)
            {
                var currentValue = ((LiteralExpressionSyntax)patternExpression).Token.ValueText;
                var regexMutantOrchestrator = new RegexMutantOrchestrator(currentValue);
                var replacementValues = regexMutantOrchestrator.Mutate();
                foreach (var regexMutation in replacementValues)
                {
                    try
                    {
                        _ = new Regex(regexMutation.ReplacementPattern);
                    }
                    catch (ArgumentException exception)
                    {
                        Logger.LogDebug($"RegexMutator created mutation {currentValue} -> {regexMutation.ReplacementPattern} which is an invalid regular expression:\n{exception.Message}");
                        continue;
                    }

                    yield return new Mutation()
                    {
                        OriginalNode = patternExpression,
                        ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(regexMutation.ReplacementPattern)),
                        DisplayName = regexMutation.DisplayName,
                        Type = Mutator.Regex,
                        Description = regexMutation.Description
                    };
                }
            }
        }
    }
}
