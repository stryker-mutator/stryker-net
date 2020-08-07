using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class StaticPropertyOrchestrator: NodeSpecificOrchestrator<PropertyDeclarationSyntax>
    {
        protected override bool CanHandleThis(PropertyDeclarationSyntax t)
        {
            return t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword) &&
                t.AccessorList != null;
        }

        internal override SyntaxNode OrchestrateMutation(PropertyDeclarationSyntax node, MutationContext context)
        {
            if (!context.MustInjectCoverageLogic)
            {
                return context.MutateChildren(node);
            }

            var trackedNode = node.TrackNodes(node.AccessorList.Accessors.Select(x => (SyntaxNode)x.Body ?? x.ExpressionBody).Where(x => x != null));
            foreach (var accessor in node.AccessorList.Accessors)
            {
                if (accessor.ExpressionBody != null)
                {
                    var markedBlock = context.Mutate(accessor.ExpressionBody);
                    trackedNode = trackedNode.ReplaceNode(trackedNode.GetCurrentNode(accessor.ExpressionBody), markedBlock);
                }
                else if (accessor.Body != null)
                {
                    var markedBlock = MutantPlacer.PlaceStaticContextMarker((BlockSyntax)context.Mutate(accessor.Body));
                    trackedNode = trackedNode.ReplaceNode(trackedNode.GetCurrentNode(accessor.Body), markedBlock);
                }
            }

            return trackedNode;
        }
    }
}
