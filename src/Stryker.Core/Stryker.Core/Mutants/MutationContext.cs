using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        public bool MustInjectCoverageLogic => mainOrchestrator.MustInjectCoverageLogic;

        public SyntaxNode Mutate(SyntaxNode subNode) => mainOrchestrator.Mutate(subNode, this);

        public SyntaxNode MutateChildren(SyntaxNode node)
        {
            return mainOrchestrator.MutateExpression(node, this);
        }
    
        public StatementSyntax MutateSubExpressionWithIfStatements(StatementSyntax originalNode, StatementSyntax nodeToReplace, ExpressionSyntax subExpression)
        {
            return mainOrchestrator.MutateSubExpressionWithIfStatements(originalNode, nodeToReplace, subExpression, this);
        }

        public MutationContext EnterStatic()
        {
            return new MutationContext(this.mainOrchestrator) { InStaticValue =  true};
        }

        public SyntaxNode MutateWithConditionals(ExpressionSyntax node, ExpressionSyntax mutateChildren)
        {
            return mainOrchestrator.MutateSubExpressionWithConditional(node, mutateChildren, this);
        }
    }
}