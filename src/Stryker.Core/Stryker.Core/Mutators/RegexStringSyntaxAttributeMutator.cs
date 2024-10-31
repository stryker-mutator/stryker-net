#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Logging;
using Stryker.Abstractions.Mutators;

namespace Stryker.Core.Mutators;

public class RegexStringSyntaxAttributeMutator : RegexMutatorBase<ArgumentSyntax>
{
    protected override ILogger Logger { get; } =
        ApplicationLogging.LoggerFactory.CreateLogger<RegexStringSyntaxAttributeMutator>();

    public override MutationLevel MutationLevel => MutationLevel.Complete;

    protected override ExpressionSyntax? GetMutateableNode(ArgumentSyntax node,
                                                           SemanticModel? semanticModel)
    {
        if (semanticModel is null)
        {
            return null;
        }

        var argumentOp =
            (semanticModel.GetOperation(node.Parent?.Parent ?? node) as IInvocationOperation)?.Arguments
        .SingleOrDefault(a => a.Syntax == node);

        if (argumentOp?.Parameter?.Type.Name is null or not "String")
        {
            return null;
        }

        return argumentOp.Parameter.GetAttributes().Any(IsRegexSyntaxAttribute) ? node.Expression : null;
    }

    private static bool IsRegexSyntaxAttribute(AttributeData a) =>
        (a.AttributeClass?.Name.Equals("StringSyntaxAttribute") ?? false) &&
        a.ConstructorArguments.FirstOrDefault().Value is StringSyntaxAttribute.Regex;
}
