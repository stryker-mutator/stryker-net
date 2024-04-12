using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Logging.StructuredLogger;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;
using Stryker.Core.Mutants.CsharpNodeOrchestrators;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class MutantPlacerTests : TestBase
    {
        public MutantPlacerTests()
        {
            codeInjection = new CodeInjection();
            placer = new MutantPlacer(new CodeInjection());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        public void MutantPlacer_ShouldPlaceWithIfStatement(int id)
        {
            var placer = new MutantPlacer(codeInjection);
            // 1 + 8;
            var originalNode = SyntaxFactory.ExpressionStatement(SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8))));

            // 1 - 8;
            var mutatedNode = SyntaxFactory.ExpressionStatement(SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8))));

            var mutants = new List<(Mutant, StatementSyntax)> { (new Mutant { Id = id, Mutation = new Mutation { ReplacementNode = mutatedNode } }, mutatedNode) };

            var result = placer.PlaceStatementControlledMutations(originalNode, mutants);

            result.ToFullString().Replace(codeInjection.HelperNamespace, "StrykerNamespace").ShouldBeSemantically("if (StrykerNamespace.MutantControl.IsActive(" + id + @"))
            {
                1 - 8;
            } else {
                1 + 8;
            }");

            var removedResult = placer.RemoveMutant(result);

            removedResult.ToString().ShouldBeSemantically(originalNode.ToString());
        }

        [Theory]
        [InlineData(10)]
        [InlineData(16)]
        public void MutantPlacer_ShouldPlaceWithConditionalExpression(int id)
        {
            var codeInjection = new CodeInjection();
            var placer = new MutantPlacer(codeInjection);
            // 1 + 8;
            var originalNode = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            // 1 - 8;
            var mutatedNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var mutants = new List<(Mutant, ExpressionSyntax)> { (new Mutant { Id = id, Mutation = new Mutation { ReplacementNode = mutatedNode } }, mutatedNode) };

            var result = placer.PlaceExpressionControlledMutations(originalNode, mutants);

            result.ToFullString()
                .ShouldBeSemantically(@$"({codeInjection.HelperNamespace}.MutantControl.IsActive({id})?1-8:1+8)");

            var removedResult = placer.RemoveMutant(result);

            removedResult.ToString().ShouldBeSemantically(originalNode.ToString());
        }
    
        [Fact]
        public void ShouldInjectInitializersAndRestore()
        {
            var source = "class Test {bool Method(out int x) {x=0;}}";
            var expected = "class Test {bool Method(out int x) {{x = default(int);}x=0;}}";

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<BlockSyntax>(source, expected,
                (n) => placer.InjectOutParametersInitialization(n,
                    new[]{SyntaxFactory.Parameter(SyntaxFactory.Identifier("x")).WithModifiers(SyntaxFactory.TokenList(new[] {SyntaxFactory.Token(SyntaxKind.OutKeyword)})).WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))
                    )}));
        }


        [Fact]
        public void ShouldStaticMarkerInStaticFieldInitializers()
        {
            var codeInjection = new CodeInjection();
            var placer = new MutantPlacer(codeInjection);
            var source = "class Test {static int x = 2;}";
            var expected = $"class Test {{static int x = {codeInjection.HelperNamespace}.MutantContext.TrackValue(()=>2);}}";

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<ExpressionSyntax>(source, expected,
                placer.PlaceStaticContextMarker, syntax => syntax.Kind() == SyntaxKind.NumericLiteralExpression);
        }

        [Fact]
        public void ShouldRollBackFailedConstructor()
        {
            var codeInjection = new CodeInjection();
            var placer = new MutantPlacer(codeInjection);
            var source = @"class Test {
static TestClass()=> Value-='a';}";

            var orchestrator = new CsharpMutantOrchestrator(placer, options: new StrykerOptions
            {
                OptimizationMode = OptimizationModes.CoverageBasedTest,
                MutationLevel = MutationLevel.Complete
            });
            var actualNode = orchestrator.Mutate(CSharpSyntaxTree.ParseText(source), null).GetRoot();

            var node = actualNode.DescendantNodes().First(t => t is BlockSyntax);

            // Remove marker
            var restored = placer.RemoveMutant(node);
            actualNode = actualNode.ReplaceNode(node, restored);

            // remove mutation
            node = actualNode.DescendantNodes().First(t => t.IsKind(SyntaxKind.IfStatement));
            restored = placer.RemoveMutant(node);
            actualNode = actualNode.ReplaceNode(node, restored);

            // remove expression to body conversion
            node = actualNode.DescendantNodes().First(t => t is ConstructorDeclarationSyntax);
            restored = placer.RemoveMutant(node);
            actualNode = actualNode.ReplaceNode(node, restored);

            var expectedNode = CSharpSyntaxTree.ParseText(source.Replace("StrykerNamespace", codeInjection.HelperNamespace));
            expectedNode.ShouldNotContainErrors();
            actualNode.SyntaxTree.ShouldBeSemantically(expectedNode);
            actualNode.SyntaxTree.ShouldNotContainErrors();
        }
    }
}
