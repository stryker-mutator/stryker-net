using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;
internal class ConstantPatternSyntaxOrchestrator: NodeSpecificOrchestrator<ConstantPatternSyntax, ConstantPatternSyntax>
{
    protected override MutationContext PrepareContext(ConstantPatternSyntax node, MutationContext context) => base.PrepareContext(node, context).BlockInjection();

    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.EnableInjection());
}
