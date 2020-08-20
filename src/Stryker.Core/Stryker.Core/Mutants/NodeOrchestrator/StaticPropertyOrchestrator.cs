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
            return t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword) && (t.ExpressionBody != null || t.AccessorList !=null);
        }

        internal override SyntaxNode OrchestrateMutation(PropertyDeclarationSyntax node, MutationContext context)
        {
            BlockSyntax mutatedAccessorBlock;
            if (!context.MustInjectCoverageLogic)
            {
                return context.MutateNodeAndChildren(node);
            }

            if (node.ExpressionBody != null)
            {
                // we need to switch to an accessor based property
                var expression = (ExpressionSyntax) context.MutateNodeAndChildren(node.ExpressionBody.Expression);
                mutatedAccessorBlock =
                    MutantPlacer.PlaceStaticContextMarker(
                        SyntaxFactory.Block(SyntaxFactory.ReturnStatement(expression)));

                return MutantPlacer.AnnotateHelper(node.Update(node.AttributeLists, node.Modifiers, node.Type,
                    node.ExplicitInterfaceSpecifier, node.Identifier,
                    SyntaxFactory.AccessorList(SyntaxFactory.List(new []{SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, mutatedAccessorBlock)})),
                    null, null, SyntaxFactory.Token(SyntaxKind.None)));
            }

            // we will adjust accessors
            var trackedNode = node.TrackNodes(node.AccessorList.Accessors);
            foreach (var accessor in node.AccessorList.Accessors)
            {
                if (accessor.ExpressionBody != null)
                {
                    var expression =
                        (ExpressionSyntax) context.MutateNodeAndChildren(accessor.ExpressionBody.Expression);
                    if (accessor.Kind() == SyntaxKind.GetAccessorDeclaration)
                    {
                        mutatedAccessorBlock =
                            MutantPlacer.PlaceStaticContextMarker(
                                SyntaxFactory.Block(
                                    SyntaxFactory.ReturnStatement(expression.WithLeadingTrivia(SyntaxFactory.Space))));
                    }
                    else
                    {
                        mutatedAccessorBlock =
                            MutantPlacer.PlaceStaticContextMarker(
                                SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(expression)));
                    }
                }
                else if (accessor.Body != null)
                {
                    mutatedAccessorBlock =
                        MutantPlacer.PlaceStaticContextMarker(
                            (BlockSyntax) context.MutateNodeAndChildren(accessor.Body));
                }
                else
                {
                    continue;
                }
                var modifiedAccessor = MutantPlacer.AnnotateHelper(accessor.Update(accessor.AttributeLists, accessor.Modifiers,
                    accessor.Keyword, mutatedAccessorBlock,
                    accessor.SemicolonToken));
                trackedNode = trackedNode.ReplaceNode(trackedNode.GetCurrentNode(accessor), modifiedAccessor);
            }

            return trackedNode;
        }
    }
}
