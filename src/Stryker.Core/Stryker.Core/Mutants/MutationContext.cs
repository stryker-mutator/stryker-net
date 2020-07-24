using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants
{
    /// <summary>
    /// Describe the (source code) context during mutation
    /// </summary>
    internal class MutationContext
    {
        private readonly MutantOrchestrator mainOrchestrator;

        public MutationContext(MutantOrchestrator mutantOrchestrator)
        {
            mainOrchestrator = mutantOrchestrator;
        }

        /// <summary>
        ///  True when inside a static initializer, fields or accessor.
        /// </summary>
        public bool InStaticValue { get; set; }

        public SyntaxNode Mutate(SyntaxNode subNode) => mainOrchestrator.Mutate(subNode, this);
    }
}