using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// We cannot control ConstructorInitializer mutation in a higher node
/// </summary>
internal class ConstructorInitializerOrchestrator : NodeSpecificOrchestrator<ConstructorInitializerSyntax, ConstructorInitializerSyntax>
{
    protected override MutationContext PrepareContext(ConstructorInitializerSyntax node, MutationContext context)
    {
        context.Leave();
        return base.PrepareContext(node, context.Enter(MutationControl.Member));
    }

    protected override void RestoreContext(MutationContext context)
    {
        base.RestoreContext(context);
        context.Leave();
        context.Enter(MutationControl.Member);
    }
}

