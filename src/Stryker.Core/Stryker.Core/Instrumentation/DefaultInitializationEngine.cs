using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation
{
    /// <summary>
    /// Injects initialization to default value for a parameter or a variable at the beginning of a method.
    /// <remarks>No check is made on the visibility of the variable.</remarks>
    /// <exception cref="InvalidOperationException">If the method has no body (virtual or expression form).</exception>
    /// </summary>
    /// <remarks>Parameters/variables are added on a one by one basis but they are all removed simultaneously.</remarks>
    internal class DefaultInitializationEngine : BaseEngine<BaseMethodDeclarationSyntax>
    {
        private static SyntaxAnnotation blockMarker = new SyntaxAnnotation("InitializationBlock");
        public DefaultInitializationEngine(string injector): base(injector)
        {
        }

        /// <summary>
        /// Add an assignment to default value for the given parameter/variable
        /// </summary>
        /// <param name="methodDeclarationSyntax">method/accessor where to inject the assignment</param>
        /// <param name="outParameterParameterName">parameter/variable name</param>
        /// <param name="outParameterParameterType">parameter/variable type</param>
        /// <returns>the method with a block containing an initialization to default for the given variables/parameters</returns>
        public BaseMethodDeclarationSyntax AddDefaultInitializer(BaseMethodDeclarationSyntax methodDeclarationSyntax, SyntaxToken outParameterParameterName, TypeSyntax outParameterParameterType)
        {
            if (methodDeclarationSyntax.Body == null)
            {
                throw new InvalidOperationException(
                    "Cant' add default initializer(s) to expression bodied or virtual method.");
            }

            IEnumerable<StatementSyntax> initializers;
            IEnumerable<StatementSyntax> originalStatements;
            if (methodDeclarationSyntax.Body.Statements.Count > 0 && methodDeclarationSyntax.Body.Statements[0].HasAnnotation(blockMarker))
            {
                // we add a new initializer, we need to get the existing ones
                initializers = (methodDeclarationSyntax.Body.Statements[0] as BlockSyntax).Statements;
                // we can skip the initializer helper block
                originalStatements = methodDeclarationSyntax.Body.Statements.Skip(1);
            }
            else
            {
                // this is the first initializer helper, no pre existing ones
                initializers = Array.Empty<StatementSyntax>();
                // keep all statements
                originalStatements = methodDeclarationSyntax.Body.Statements;
            }

            var initializer = SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(outParameterParameterName),
                outParameterParameterType.BuildDefaultExpression()));
            var initializersBlock = SyntaxFactory.Block(initializers.Append(initializer))
                .WithAdditionalAnnotations(blockMarker);

            return methodDeclarationSyntax.WithBody(methodDeclarationSyntax.Body.WithStatements(new SyntaxList<StatementSyntax>(originalStatements.Prepend(initializersBlock)))).WithAdditionalAnnotations(Marker);
        }

        /// <summary>
        /// Removes all initializer from the given method.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>the method with the initialization block removed.</returns>
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
    }
}
