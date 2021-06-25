using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StaticFieldDeclarationOrchestrator: NodeSpecificOrchestrator<FieldDeclarationSyntax, BaseFieldDeclarationSyntax>
    {
        protected override bool CanHandle(FieldDeclarationSyntax t) => t.IsStatic();
        protected override MutationContext PrepareContext(FieldDeclarationSyntax _, MutationContext context) => context.EnterStatic();

        protected override BaseFieldDeclarationSyntax InjectMutations(FieldDeclarationSyntax sourceNode, BaseFieldDeclarationSyntax targetNode,
            MutationContext context)
        {
            var result = base.InjectMutations(sourceNode, targetNode, context);

            result = result.ReplaceNodes(result.Declaration.Variables.Where(v => v.Initializer != null).Select(v => v.Initializer.Value),
                (syntax, _) => MutantPlacer.PlaceStaticContextMarker(syntax));

            return result;
        }
    }
}
