using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Instrumentation
{    /// <summary>
    /// Injects a mutation controlled by an if Statement.
    /// </summary>
    internal class IfInstrumentationEngine: BaseEngine<IfStatementSyntax>
    {
        public IfInstrumentationEngine(string annotation) : base(annotation)
        {
        }

        /// <summary>
        /// Injects an if statement with the original code or the mutated one, depending on condition's result.
        /// </summary>
        /// <param name="condition">Expression for the condition.</param>
        /// <param name="original">Original code</param>
        /// <param name="mutated">Mutated code</param>
        /// <returns>A statement containing the expected construct.</returns>
        /// <remarks>This method works with statement and block.</remarks>
        public IfStatementSyntax InjectIf(ExpressionSyntax condition, StatementSyntax originalNode, StatementSyntax mutatedNode)
        {
            return SyntaxFactory.IfStatement(condition, 
                AsBlock(mutatedNode), 
                SyntaxFactory.ElseClause(AsBlock(originalNode))).WithAdditionalAnnotations(Marker);
        }

        private static BlockSyntax AsBlock(StatementSyntax code)
        {
            return (code as BlockSyntax) ?? SyntaxFactory.Block(code);
        }

        /// <summary>
        /// Returns the original code.
        /// </summary>
        /// <param name="ifNode">if statement to be 'removed'</param>
        /// <returns>the original node.</returns>
        /// <remarks>this method returns either a single statement or a syntax block.</remarks>
        protected override SyntaxNode Revert(IfStatementSyntax ifNode)
        {
            if (ifNode.Else?.Statement is BlockSyntax block)
            {
                return block.Statements.Count == 1 ? block.Statements[0] : block;
            }
            throw new InvalidOperationException($"Expected a block containing an 'else' statement, found:\n{ifNode.ToFullString()}.");
        }
    }
}
