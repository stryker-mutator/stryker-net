using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation
{
    internal class DefaultInitializationEngine : BaseEngine<BaseMethodDeclarationSyntax>
    {
        private static SyntaxAnnotation blockMarker = new SyntaxAnnotation("InitializationBlock");
        public DefaultInitializationEngine(string injector): base(injector)
        {
        }

        protected override SyntaxNode Revert(BaseMethodDeclarationSyntax node)
        {
            if (node.Body == null || node.Body.Statements.Count == 0 || node.Body.Statements[0].Kind() != SyntaxKind.Block)
            {
                throw new InvalidOperationException(
                    "Can't find initializer block at the beginning of method.");
            }

            return node.WithBody(
                    node.Body.WithStatements(new SyntaxList<StatementSyntax>(node.Body.Statements.Skip(1))))
                .WithoutAnnotations(Marker);
        }

        public BaseMethodDeclarationSyntax AddDefaultInitializer(BaseMethodDeclarationSyntax node, SyntaxToken outParameterParameterName, TypeSyntax outParameterParameterType)
        {
            if (node.Body == null)
            {
                throw new InvalidOperationException(
                    "Cant' add default initializer(s) to expression bodied or virtual method.");
            }

            IEnumerable<StatementSyntax> initializers = null;
            IEnumerable<StatementSyntax> originalStatements = null;
            if (node.Body.Statements.Count > 0 && node.Body.Statements[0].HasAnnotation(blockMarker))
            {
                // we add a new initializer, we need to get the existing ones
                initializers = (node.Body.Statements[0] as BlockSyntax).Statements;
                // we can skip the initializer helper block
                originalStatements = node.Body.Statements.Skip(1);
            }
            else
            {
                // this is the first initializer helper, no pre existing ones
                initializers = Array.Empty<StatementSyntax>();
                // keep all statements
                originalStatements = node.Body.Statements;
            }

            var initializer = SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(outParameterParameterName),
                outParameterParameterType.BuildDefaultExpression()));
            var initializersBlock = SyntaxFactory.Block(initializers.Append(initializer))
                .WithAdditionalAnnotations(blockMarker);

            return node.WithBody(node.Body.WithStatements(new SyntaxList<StatementSyntax>(originalStatements.Prepend(initializersBlock)))).WithAdditionalAnnotations(Marker);
        }
    }
}
