using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    /// <summary>
    /// Supports static constructor.
    /// </summary>
    internal class StaticConstructorOrchestrator : BaseMethodDeclarationOrchestrator<ConstructorDeclarationSyntax>
    {
        protected override bool CanHandle(ConstructorDeclarationSyntax t) => t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);

        /// <inheritdoc/>
        /// <remarks>Injects a static marker used for coverage information; this implies converting
        /// expression arrow bodied method to regular ones.</remarks>
        protected override BaseMethodDeclarationSyntax InjectMutations(ConstructorDeclarationSyntax sourceNode,
            BaseMethodDeclarationSyntax targetNode, MutationContext context)
        {
            var mutated = base.InjectMutations(sourceNode, targetNode, context);

            if (!context.MustInjectCoverageLogic)
            {
                return mutated;
            }
            if (mutated.ExpressionBody != null)
            {
                // we need a body to place the marker
                mutated = MutantPlacer.ConvertExpressionToBody(mutated);
            }

            return mutated.ReplaceNode(mutated.Body!, MutantPlacer.PlaceStaticContextMarker(mutated.Body));
        }
    }
}
