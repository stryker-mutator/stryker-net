using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation
{
    internal static class EngineHelpers
    {
        public static BlockSyntax InjectReturn(BlockSyntax block, TypeSyntax returnType, SyntaxTokenList modifiers)
        {
            // if we had no body or the the last statement was a return, no need to add one
            if (block == null || returnType == null || block.Statements.Count == 0 || block!.Statements.Last().Kind() == SyntaxKind.ReturnStatement || returnType.IsVoid())
            {
                return null;
            }

            // we can also skip iterator methods, as they don't need to end with return
            if (block.ContainsNodeThatVerifies(x => x.IsKind(SyntaxKind.YieldReturnStatement) || x.IsKind(SyntaxKind.YieldBreakStatement), false))
            {
                // not need to add yield return at the end of an enumeration method
                return null;
            }

            // the GenericNameSyntax node can be encapsulated by QualifiedNameSyntax nodes
            var genericReturn = returnType.DescendantNodesAndSelf().OfType<GenericNameSyntax>().FirstOrDefault();
            if (modifiers.Any(x => x.IsKind(SyntaxKind.AsyncKeyword)))
            {
                if (genericReturn != null)
                {
                    // if the method is async and returns a generic task, make the return default return the underlying type
                    returnType = genericReturn.TypeArgumentList.Arguments.First();
                }
                else
                {
                    // if the method is async but returns a non-generic task, don't add the return default
                    return null;
                }
            }

            return block.AddStatements(
                SyntaxFactory.ReturnStatement(returnType.BuildDefaultExpression()));
        }

        public static BlockSyntax RemoveReturn(BlockSyntax block)
        {
            if (block?.Statements.Last().IsKind(SyntaxKind.ReturnStatement) != true)
            {
                throw new InvalidOperationException($"No return at the end of: {block}");
            }

            return block.WithStatements(block.Statements.Remove(block.Statements.Last()));
        }
    }
}
