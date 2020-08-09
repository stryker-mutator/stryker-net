using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class MethodDeclarationOrchestrator : NodeSpecificOrchestrator<MethodDeclarationSyntax>
    {
        protected override bool CanHandleThis(MethodDeclarationSyntax t)
        {
            return t.Body != null;
        }

        internal override SyntaxNode OrchestrateMutation(MethodDeclarationSyntax node, MutationContext context)
        {
            node = (MethodDeclarationSyntax) context.MutateChildren(node);

            // If method return type is void skip the node
            if (node.ReturnType is PredefinedTypeSyntax predefinedType &&
                predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                return node;
            }

            var returnType = node.ReturnType;

            // the GenericNameSyntax node can be encapsulated by QualifiedNameSyntax nodes
            var genericReturn = returnType.DescendantNodesAndSelf().OfType<GenericNameSyntax>().FirstOrDefault();
            if (node.Modifiers.Any(x => x.IsKind(SyntaxKind.AsyncKeyword)))
            {
                if (genericReturn != null)
                {
                    // if the method is async and returns a generic task, make the return default return the underlying type
                    returnType = genericReturn.TypeArgumentList.Arguments.First();
                }
                else
                {
                    // if the method is async but returns a non-generic task, don't add the return default
                    return node;
                }
            }

            var newBody = node.Body.AddStatements(
                MutantPlacer.AnnotateHelper(
                    SyntaxFactory.ReturnStatement(SyntaxFactory.DefaultExpression(returnType).WithLeadingTrivia(SyntaxFactory.Space))));
            node = node.ReplaceNode(node.Body, newBody);

            return node;
        }
    }
}