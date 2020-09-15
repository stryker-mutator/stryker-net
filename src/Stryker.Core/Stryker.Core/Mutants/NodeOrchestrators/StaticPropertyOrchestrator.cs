using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StaticPropertyOrchestrator: NodeSpecificOrchestrator<PropertyDeclarationSyntax>
    {
        protected override bool CanHandle(PropertyDeclarationSyntax t)
        {
            return t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword) &&
                t.AccessorList != null;
        }

        internal override SyntaxNode OrchestrateMutation(PropertyDeclarationSyntax node, MutationContext context)
        {
            //if (!context.MustInjectCoverageLogic)
            {
                return context.MutateNodeAndChildren(node);
            }

            var trackedNode = node.TrackNodes(node.AccessorList.Accessors.Select(x => (SyntaxNode)x.Body ?? x.ExpressionBody).Where(x => x != null));
            foreach (var accessor in node.AccessorList.Accessors)
            {
                if (accessor.ExpressionBody != null)
                {
                    var markedBlock = context.MutateNodeAndChildren(accessor.ExpressionBody);
                    trackedNode = trackedNode.ReplaceNode(trackedNode.GetCurrentNode(accessor.ExpressionBody), markedBlock);
                }
                else if (accessor.Body != null)
                {
                    var markedBlock = MutantPlacer.PlaceStaticContextMarker((BlockSyntax)context.MutateNodeAndChildren(accessor.Body));
                    trackedNode = trackedNode.ReplaceNode(trackedNode.GetCurrentNode(accessor.Body), markedBlock);
                }
            }

            return trackedNode;
        }
    }
}
