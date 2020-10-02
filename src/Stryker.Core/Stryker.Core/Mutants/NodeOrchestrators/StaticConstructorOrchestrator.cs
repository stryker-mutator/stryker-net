using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StaticConstructorOrchestrator : BaseMethodDeclarationOrchestrator<ConstructorDeclarationSyntax>
    {
        protected override bool NewContext => true;

        protected override bool CanHandle(ConstructorDeclarationSyntax t)
        {
            return t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);
        }

        protected override BaseMethodDeclarationSyntax InjectMutations(ConstructorDeclarationSyntax originalNode,
            BaseMethodDeclarationSyntax mutatedNode, MutationContext context)
        {
            var mutated = base.InjectMutations(originalNode, mutatedNode, context);

            if (!context.MustInjectCoverageLogic)
            {
                return mutated;
            }
            if (mutated.ExpressionBody != null)
            {
                // we need a body to place the marker
                mutated = MutantPlacer.ConvertExpressionToBody(mutated);
            }

            return mutated.ReplaceNode(mutated.Body!, MutantPlacer.PlaceStaticContextMarker(mutated.Body));
        }

        public StaticConstructorOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}