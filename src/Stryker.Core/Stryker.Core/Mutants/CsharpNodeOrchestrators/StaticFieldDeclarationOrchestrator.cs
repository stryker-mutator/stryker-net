using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Ensure static Fields are marked as static
/// </summary>
internal class StaticFieldDeclarationOrchestrator : MemberDefinitionOrchestrator<FieldDeclarationSyntax>
{
    protected override bool CanHandle(FieldDeclarationSyntax t) => t.IsStatic() && t.Declaration.Variables.Any(v => v.Initializer != null);

    protected override MutationContext PrepareContext(FieldDeclarationSyntax node, MutationContext context) => base.PrepareContext(node, context).EnterStatic();

    protected override FieldDeclarationSyntax InjectMutations(FieldDeclarationSyntax sourceNode, FieldDeclarationSyntax targetNode,
        SemanticModel semanticModel, MutationContext context)
    {
        // mutate the node normally
        var result = base.InjectMutations(sourceNode, targetNode, semanticModel, context);
        if (!context.MustInjectCoverageLogic)
        {
            return result;
        }

        // inject static marker logic. Note that we need to perform this AFTER mutation has been done, otherwise mutation injection may fail
        return result.ReplaceNodes(result.Declaration.Variables.Where(v => v.Initializer != null).Select(v => v.Initializer.Value),
            (syntax, _) => context.PlaceStaticContextMarker(syntax));
    }
}
