using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class MemberDeclarationOrchestrator<T, TBase> : NodeSpecificOrchestrator<T, TBase> where T : TBase where TBase : MemberDeclarationSyntax
    {
        public MemberDeclarationOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        protected override TBase OrchestrateChildrenMutation(T node, MutationContext context)
        {
            var result = base.OrchestrateChildrenMutation(node, context);
            // discard any mutations that has not been injected in the code yet
            // otherwise they will end up in some other method/properties.
            // Note:
            // 1 - this is sure sign that current design needs to be improved
            // 2 - this logic needs to be updated if we can control mutations at some higher level
            context.Discard(); 
            return result;
        }
    }
}
