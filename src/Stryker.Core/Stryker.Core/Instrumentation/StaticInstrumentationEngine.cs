using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.InjectedHelpers;

namespace Stryker.Core.Instrumentation
{
    /// <summary>
    /// Injects a static marker, to help identification of mutations executed through a static constructor/method/property...
    /// </summary>
    internal class StaticInstrumentationEngine : BaseEngine<BlockSyntax>
    {
        /// <summary>
        /// injects a 'using' block with static marker class used by coverage logic.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="injector"></param>
        /// <returns></returns>
        public BlockSyntax PlaceStaticContextMarker(BlockSyntax block, CodeInjection injector) =>
            SyntaxFactory.Block( 
                SyntaxFactory.UsingStatement(null, injector.GetContextClassConstructor(), block)).WithAdditionalAnnotations(Marker);
    
        protected override SyntaxNode Revert(BlockSyntax node)
        {
            if ( node.Statements.Count == 1 && node.Statements[0] is UsingStatementSyntax usingStatement)
            {
                return usingStatement.Statement;
            }

            return node;
        }
    }
}
