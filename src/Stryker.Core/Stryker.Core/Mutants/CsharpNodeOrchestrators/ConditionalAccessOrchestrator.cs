using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class ConditionalAccessOrchestrator: NodeSpecificOrchestrator<ConditionalAccessExpressionSyntax, ExpressionSyntax>
{
    /// <inheritdoc/>
    /// <remarks>Inject all pending mutations controlled with conditional operator(s).</remarks>
    protected override ExpressionSyntax InjectMutations(ConditionalAccessExpressionSyntax sourceNode, ExpressionSyntax targetNode, SemanticModel semanticModel, MutationContext context) => context.InjectExpressionLevel(targetNode, sourceNode);

    protected override ExpressionSyntax OrchestrateChildrenMutation(ConditionalAccessExpressionSyntax node, SemanticModel semanticModel, MutationContext context)
    {
        MutateSingleNode(node.Expression, semanticModel, context.EnterSubExpression());
        var result = node.ReplaceNode(node.WhenNotNull,MutateSingleNode(node.WhenNotNull, semanticModel, context));
        context.Leave(MutationControl.Expression);
        return result;
    }

    protected override MutationContext StoreMutations(ConditionalAccessExpressionSyntax node,
        IEnumerable<Mutant> mutations,
        MutationContext context)
    {
        // if the expression contains a declaration, it must be controlled at the block level.
        if (node.ContainsDeclarations())
        {
            context.AddBlockLevel(mutations);
        }
        else
        {
            context.AddExpressionLevel(mutations);
        }
        return context;
    }
    }
