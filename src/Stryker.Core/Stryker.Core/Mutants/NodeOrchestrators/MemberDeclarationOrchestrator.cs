using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class MemberDeclarationOrchestrator<T, TBase> : NodeSpecificOrchestrator<T, TBase> where T : TBase where TBase : MemberDeclarationSyntax
    {
        public MemberDeclarationOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        protected override TBase OrchestrateChildrenMutation(T node, MutationContext context) => base.OrchestrateChildrenMutation(node, context);
    }
}
