using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.InjectedHelpers;

namespace Stryker.Core.Instrumentation
{
    class StaticInstrumentationEngine : IInstrumentCode
    {
        private readonly SyntaxAnnotation _marker;
        public string IInstrumentEngineID => "StaticMarker";

        private readonly ExpressionSyntax _cachedMarker = SyntaxFactory.ParseExpression(CodeInjection.StaticMarker);

        public StaticInstrumentationEngine(string annotation)
        {
            _marker = new SyntaxAnnotation(annotation, IInstrumentEngineID);
        }

        public SyntaxNode RemoveInstrumentation(SyntaxNode node)
        {
            if (node is BlockSyntax block && block.Statements.Count == 1 && block.Statements[0] is UsingStatementSyntax usingStatement)
            {
                return usingStatement.Statement;
            }
            throw new InvalidOperationException($"Expected a block containing an 'using' statement, found:\n{node.ToFullString()}.");
        }

        public BlockSyntax PlaceStaticContextMarker(StatementSyntax block) =>
            SyntaxFactory.Block( 
                SyntaxFactory.UsingStatement(null, _cachedMarker, block)).WithAdditionalAnnotations(_marker);

    }
}
