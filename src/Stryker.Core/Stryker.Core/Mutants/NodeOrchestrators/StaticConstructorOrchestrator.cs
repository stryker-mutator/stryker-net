using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        protected override MutationContext PrepareContext(ConstructorDeclarationSyntax node, MutationContext context)
        {
            if (!context.MustInjectCoverageLogic)
            {
                return context.EnterStatic();
            }

            return context;
        }

        protected override SyntaxNode OrchestrateMutation(ConstructorDeclarationSyntax node, MutationContext context)
        {
            if (!context.MustInjectCoverageLogic)
            {
                return context.MutateNodeAndChildren(node);
            }
            var trackedConstructor = node.TrackNodes((SyntaxNode) node.Body ?? node.ExpressionBody);
            if (node.ExpressionBody != null)
            {
                var mutated = node.ReplaceNode(node.ExpressionBody, context.MutateNodeAndChildren(node.ExpressionBody));
                trackedConstructor = MutantPlacer.ConvertExpressionToBody(mutated);
                trackedConstructor = trackedConstructor.ReplaceNode(trackedConstructor.Body, MutantPlacer.PlaceStaticContextMarker(trackedConstructor.Body));
            }
            else if (node.Body != null)
            {
                var markedBlock = MutantPlacer.PlaceStaticContextMarker((BlockSyntax) context.MutateNodeAndChildren(node.Body));
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