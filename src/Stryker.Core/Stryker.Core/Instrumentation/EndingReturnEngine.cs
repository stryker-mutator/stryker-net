using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation
{
    /// <summary>
    /// Injects 'return default(...)' statement at the end of a method
    /// </summary>
    internal class EndingReturnEngine: BaseEngine<BaseMethodDeclarationSyntax>
    {
        public EndingReturnEngine(string markerId) : base(markerId)
        {
        }

        public BaseMethodDeclarationSyntax InjectReturn(BaseMethodDeclarationSyntax method)
        {
            // if we had no body or the the last statement was a return, no need to add one
            if (method.Body == null || method.Body.Statements.Count == 0 || method.Body!.Statements.Last().Kind() == SyntaxKind.ReturnStatement || !method.NeedsReturn())
            {
                return method;
            }

            // we can also skip iterator methods, as they don't need to end with return
            if (method.Body.ContainsNodeThatVerifies(x => x.IsKind(SyntaxKind.YieldReturnStatement) || x.IsKind(SyntaxKind.YieldBreakStatement), false))
            {
                // not need to add yield return at the end of an enumeration method
                return method;
            }

            var returnType = method.ReturnType();

            // the GenericNameSyntax node can be encapsulated by QualifiedNameSyntax nodes
            var genericReturn = returnType.DescendantNodesAndSelf().OfType<GenericNameSyntax>().FirstOrDefault();
            if (method.Modifiers.Any(x => x.IsKind(SyntaxKind.AsyncKeyword)))
            {
                if (genericReturn != null)
                {
                    // if the method is async and returns a generic task, make the return default return the underlying type
                    returnType = genericReturn.TypeArgumentList.Arguments.First();
                }
                else
                {
                    // if the method is async but returns a non-generic task, don't add the return default
                    return method;
                }
            }

            method = method.WithBody(method.Body!.AddStatements(
                    SyntaxFactory.ReturnStatement(returnType.BuildDefaultExpression()))).WithAdditionalAnnotations(Marker);

            return method;
        }

        protected override SyntaxNode Revert(BaseMethodDeclarationSyntax node)
        {
            if (node.Body?.Statements.Last().IsKind(SyntaxKind.ReturnStatement) != true)
            {
                throw new InvalidOperationException($"No return at the end of: {node.Body}");
            }

            return node.WithBody(node.Body.WithStatements(node.Body.Statements.Remove(node.Body.Statements.Last()))).WithoutAnnotations(Marker);
        }
    }
}
