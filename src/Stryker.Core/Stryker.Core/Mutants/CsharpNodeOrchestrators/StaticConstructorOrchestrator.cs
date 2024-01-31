using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Supports static constructor.
/// </summary>
internal class StaticConstructorOrchestrator : BaseMethodDeclarationOrchestrator<ConstructorDeclarationSyntax>
{
    protected override bool CanHandle(ConstructorDeclarationSyntax t) => t.IsStatic();

    protected override MutationContext PrepareContext(ConstructorDeclarationSyntax node, MutationContext context) => base.PrepareContext(node, context).EnterStatic();

    /// <inheritdoc/>
    /// <remarks>Injects a static marker used for coverage information; this implies converting
    /// expression arrow bodied method to regular ones.</remarks>
    protected override ConstructorDeclarationSyntax InjectMutations(ConstructorDeclarationSyntax sourceNode,
        ConstructorDeclarationSyntax targetNode, SemanticModel semanticModel, MutationContext context)
    {
        var mutated = base.InjectMutations(sourceNode, targetNode, semanticModel, context);

        if (!context.MustInjectCoverageLogic)
        {
            return mutated;
        }

        if (mutated.ExpressionBody != null)
        {
            // we need a body to place the marker
            mutated = ConvertToBlockBody(mutated, RoslynHelper.VoidTypeSyntax());
        }

        return mutated.ReplaceNode(mutated.Body!, context.PlaceStaticContextMarker(mutated.Body));
    }
}
