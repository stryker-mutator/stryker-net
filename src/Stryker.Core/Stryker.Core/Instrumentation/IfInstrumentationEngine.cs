using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Instrumentation
{
    class IfInstrumentationEngine: IInstrumentCode
    {
        private readonly SyntaxAnnotation _marker;

        public IfInstrumentationEngine(string annotation)
        {
            _marker = new SyntaxAnnotation(annotation, InstrumentEngineID);
        }

        public string InstrumentEngineID => "IfInstrumentation";

        public IfStatementSyntax InjectIf(ExpressionSyntax condition, StatementSyntax originalNode, StatementSyntax mutatedNode)
        {
            return SyntaxFactory.IfStatement(condition, 
                AsBlock(mutatedNode), 
                SyntaxFactory.ElseClause(AsBlock(originalNode))).WithAdditionalAnnotations(_marker);
        }

        private static BlockSyntax AsBlock(StatementSyntax code)
        {
            return (code as BlockSyntax) ?? SyntaxFactory.Block(code);
        }

        public SyntaxNode RemoveInstrumentation(SyntaxNode node)
        {
            if (node is IfStatementSyntax ifNode && ifNode.Else?.Statement is BlockSyntax block)
            {
                return block.Statements.Count == 1 ? block.Statements[0] : block;
            }
            throw new InvalidOperationException($"Expected a block containing an 'if' statement, found:\n{node.ToFullString()}.");
        }
    }
}
