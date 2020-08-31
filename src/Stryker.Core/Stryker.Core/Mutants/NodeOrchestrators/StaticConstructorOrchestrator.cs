using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StaticConstructorOrchestrator : NodeSpecificOrchestrator<ConstructorDeclarationSyntax>
    {
        protected override bool CanHandle(ConstructorDeclarationSyntax t)
        {
            return t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);
        }

        internal override SyntaxNode OrchestrateMutation(ConstructorDeclarationSyntax node, MutationContext context)
        {
            if (!context.MustInjectCoverageLogic)
            {
                return context.EnterStatic().MutateNodeAndChildren(node);
            }
            var trackedConstructor = node.TrackNodes((SyntaxNode) node.Body ?? node.ExpressionBody);
            if (node.ExpressionBody != null)
            {
                var bodyBlock =
                    SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(node.ExpressionBody.Expression));
                var markedBlock = MutantPlacer.PlaceStaticContextMarker((BlockSyntax) context.MutateNodeAndChildren(bodyBlock));
                trackedConstructor = trackedConstructor.Update(
                    trackedConstructor.AttributeLists,
                    trackedConstructor.Modifiers,
                    trackedConstructor.Identifier,
                    trackedConstructor.ParameterList,
                    trackedConstructor.Initializer,
                    markedBlock,
                    null,
                    SyntaxFactory.Token(SyntaxKind.None));
            }
            else if (node.Body != null)
            {
                var markedBlock = MutantPlacer.PlaceStaticContextMarker((BlockSyntax) context.MutateNodeAndChildren(node.Body));
                trackedConstructor =
                    trackedConstructor.ReplaceNode(trackedConstructor.GetCurrentNode(node.Body), markedBlock);
            }
            return trackedConstructor;

        }
    }
}