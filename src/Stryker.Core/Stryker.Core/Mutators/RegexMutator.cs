#nullable enable
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Logging;
using Stryker.Abstractions.Mutators;

namespace Stryker.Core.Mutators;

public class RegexMutator : RegexMutatorBase<ObjectCreationExpressionSyntax>
{
    private const string PatternArgumentName = "pattern";

    protected override ILogger Logger { get; } = ApplicationLogging.LoggerFactory.CreateLogger<RegexMutator>();

    public override MutationLevel MutationLevel => MutationLevel.Advanced;

    protected override ExpressionSyntax? GetMutateableNode(ObjectCreationExpressionSyntax node,
                                                           SemanticModel?                 semanticModel)
    {
        var name = node.Type.ToString();

        if (name != nameof(Regex) && name != typeof(Regex).FullName)
        {
            return null;
        }

        var arguments = node.ArgumentList?.Arguments;

        var namedArgument = arguments?.FirstOrDefault(static argument =>
                                                          argument.NameColon?.Name.Identifier.ValueText ==
                                                          PatternArgumentName);
        var patternArgument = namedArgument ?? node.ArgumentList?.Arguments.FirstOrDefault();
        return patternArgument?.Expression;
    }
}
