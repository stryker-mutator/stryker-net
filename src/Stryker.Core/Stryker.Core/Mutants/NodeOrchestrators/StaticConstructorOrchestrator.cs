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

        protected override SyntaxNode OrchestrateMutation(ConstructorDeclarationSyntax node, MutationContext context)
        {
            if (!context.MustInjectCoverageLogic)
            {
                return base.OrchestrateMutation(node, context.EnterStatic());
            }
            var trackedConstructor = node.TrackNodes((SyntaxNode) node.Body ?? node.ExpressionBody);
            if (node.ExpressionBody != null)
            {
                var mutated = node.ReplaceNode(node.ExpressionBody, MutantOrchestrator.Mutate(node.ExpressionBody, context));
                trackedConstructor = MutantPlacer.ConvertExpressionToBody(mutated);
                trackedConstructor = trackedConstructor.ReplaceNode(trackedConstructor.Body, MutantPlacer.PlaceStaticContextMarker(trackedConstructor.Body));
            }
            else if (node.Body != null)
            {
                var markedBlock = MutantPlacer.PlaceStaticContextMarker((BlockSyntax) MutantOrchestrator.Mutate(node.Body, context));
                trackedConstructor =
                    trackedConstructor.ReplaceNode(trackedConstructor.GetCurrentNode(node.Body), markedBlock);
            }
            return trackedConstructor;
        }

        public StaticConstructorOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}