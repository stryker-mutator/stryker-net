using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.InjectedHelpers;

namespace Stryker.Core.Instrumentation
{
    internal class StaticInstrumentationEngine : BaseEngine<BlockSyntax>
    {
        private readonly ExpressionSyntax _cachedMarker = SyntaxFactory.ParseExpression(CodeInjection.StaticMarker);

        public StaticInstrumentationEngine(string annotation) : base(annotation)
        {
        }

        protected override SyntaxNode Revert(BlockSyntax node)
        {
            if ( node.Statements.Count == 1 && node.Statements[0] is UsingStatementSyntax usingStatement)
            {
                return usingStatement.Statement;
            }

            return node;
        }

        public BlockSyntax PlaceStaticContextMarker(BlockSyntax block) =>
            SyntaxFactory.Block( 
                SyntaxFactory.UsingStatement(null, _cachedMarker, block)).WithAdditionalAnnotations(Marker);

    }
}
