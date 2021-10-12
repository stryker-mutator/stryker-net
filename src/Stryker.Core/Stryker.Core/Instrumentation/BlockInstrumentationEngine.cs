using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Instrumentation
{
    /// <summary>
    /// Injects block mutations
    /// </summary>
    internal class BlockInstrumentationEngine : BaseEngine<BlockSyntax>
    {
        /// <summary>
        /// Injects an if statement inside a block with the original code or the mutated one, depending on condition's result.
        /// </summary>
        /// <param name="condition">Expression for the condition.</param>
        /// <param name="originalNode">Original code</param>
        /// <param name="mutatedNode">Mutated code</param>
        /// <returns>A statement containing the expected construct.</returns>
        /// <remarks>This method works with statement and block.</remarks>
        public BlockSyntax PlaceBlockMutation(ExpressionSyntax condition, BlockSyntax originalNode, BlockSyntax mutatedNode) =>
            SyntaxFactory.Block(SyntaxFactory.IfStatement(condition, mutatedNode, SyntaxFactory.ElseClause(originalNode)).WithAdditionalAnnotations(Marker));

        /// <summary>
        /// Returns the original code.
        /// </summary>
        /// <param name="block">if statement to be 'removed'</param>
        /// <returns>the original node.</returns>
        /// <remarks>this method returns a syntax block.</remarks>
        protected override SyntaxNode Revert(BlockSyntax mutatedBlock)
        {
            if (mutatedBlock.Statements.FirstOrDefault() is IfStatementSyntax ifStatement && ifStatement.Else?.Statement is BlockSyntax block)
            {
                return block.Statements.Count == 1 ? block.Statements[0] : block;
            }
            throw new InvalidOperationException($"Expected a block containing an 'if' statement, found:\n{mutatedBlock.ToFullString()}.");
        }
    }
}
