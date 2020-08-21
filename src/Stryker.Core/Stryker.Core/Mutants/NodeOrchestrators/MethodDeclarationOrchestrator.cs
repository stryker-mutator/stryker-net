using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class MethodDeclarationOrchestrator : NodeSpecificOrchestrator<MethodDeclarationSyntax>
    {
        protected override bool CanHandle(MethodDeclarationSyntax t)
        {
            return t.Body != null;
        }

        internal override SyntaxNode OrchestrateMutation(MethodDeclarationSyntax node, MutationContext context)
        {
            var mutatedNode = (MethodDeclarationSyntax) context.MutateNodeAndChildren(node);

            if (mutatedNode.Body == null)
            {
                return mutatedNode;
            }
            // If method return type is void skip the node
            if (mutatedNode.ReturnType is PredefinedTypeSyntax predefinedType &&
                predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                return mutatedNode;
            }

            if (mutatedNode.Body.Statements.Last().Kind() == SyntaxKind.ReturnStatement)
            {
                return mutatedNode;
            }

            var returnType = mutatedNode.ReturnType;

            // the GenericNameSyntax node can be encapsulated by QualifiedNameSyntax nodes
            var genericReturn = returnType.DescendantNodesAndSelf().OfType<GenericNameSyntax>().FirstOrDefault();
            if (mutatedNode.Modifiers.Any(x => x.IsKind(SyntaxKind.AsyncKeyword)))
            {
                if (genericReturn != null)
                {
                    // if the method is async and returns a generic task, make the return default return the underlying type
                    returnType = genericReturn.TypeArgumentList.Arguments.First();
                }
                else
                {
                    // if the method is async but returns a non-generic task, don't add the return default
                    return mutatedNode;
                }
            }

            var newBody = mutatedNode.Body.AddStatements(
                MutantPlacer.AnnotateHelper(
                    SyntaxFactory.ReturnStatement(SyntaxFactory.DefaultExpression(returnType).WithLeadingTrivia(SyntaxFactory.Space))));
            mutatedNode = mutatedNode.ReplaceNode(mutatedNode.Body, newBody);

            return mutatedNode;
        }
    }
}