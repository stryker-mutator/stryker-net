using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class MutantPlacerTests : TestBase
    {
        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        public void MutantPlacer_ShouldPlaceWithIfStatement(int id)
        {
            var codeInjection = new CodeInjection();
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

            var removedResult = MutantPlacer.RemoveMutant(result);

            removedResult.ToString().ShouldBeSemantically(originalNode.ToString());
        }

        private void CheckMutantPlacerProperlyPlaceAndRemoveHelpers<T>(string sourceCode, string expectedCode,
            Func<T, T> placer, Predicate<T> condition = null) where T : SyntaxNode =>
            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<T, T>(sourceCode, expectedCode, placer, condition);

        private void CheckMutantPlacerProperlyPlaceAndRemoveHelpers<T, TU>(string sourceCode, string expectedCode,
            Func<T, T> placer, Predicate<T> condition = null) where T : SyntaxNode where TU : SyntaxNode
        {
            var actualNode = CSharpSyntaxTree.ParseText(sourceCode).GetRoot();

            var node = actualNode.DescendantNodes().First(t => t is T ct && (condition == null || condition(ct))) as T;
            // inject helper
            actualNode = actualNode.ReplaceNode(node, placer(node));
            actualNode.ToFullString().ShouldBeSemantically(expectedCode);

            TU newNode;
            if (typeof(TU) == typeof(T))
            {
                newNode = actualNode.DescendantNodes().First(t => t is TU && t.ContainsAnnotations) as TU;
            }
            else
            {
                newNode = actualNode.DescendantNodes().First(t => t is T).DescendantNodes().First(t => t is TU && t.ContainsAnnotations) as TU;
            }

            // Remove helper
            var restored = MutantPlacer.RemoveMutant(newNode);
            actualNode = actualNode.ReplaceNode(newNode, restored);
            actualNode.ToFullString().ShouldBeSemantically(sourceCode);
            // try to remove again
            Should.Throw<InvalidOperationException>(() => MutantPlacer.RemoveMutant(restored));
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

            var removedResult = MutantPlacer.RemoveMutant(result);

            removedResult.ToString().ShouldBeSemantically(originalNode.ToString());
        }

        [Theory]
        [InlineData("static TestClass()=> Value-='a';", "static TestClass(){ Value-='a';}")]
        [InlineData("void TestClass()=> Value-='a';", "void TestClass(){ Value-='a';}")]
        [InlineData("int TestClass()=> 1;", "int TestClass(){ return 1;}")]
        [InlineData("~TestClass()=> Value-='a';", "~TestClass(){ Value-='a';}")]
        [InlineData("public static operator int(Test t)=> 0;", "public static operator int(Test t){ return 0;}")]
        [InlineData("public static int operator +(Test t, Test q)=> 0;", "public static int operator +(Test t, Test q){return 0;}")]
        public void ShouldConvertExpressionBodyBackAndForth(string original, string injected)
        {
            var source = $"class Test {{{original}}}";
            var expectedCode = $"class Test {{{injected}}}";

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<BaseMethodDeclarationSyntax>(source, expectedCode, MutantPlacer.ConvertExpressionToBody);
        }

        [Theory]
        [InlineData("void TestClass(){ void LocalFunction() => Value-='a';}", "void TestClass(){ void LocalFunction() {Value-='a';};}}")]
        public void ShouldConvertExpressionBodyBackLocalFunctionAndForth(string original, string injected)
        {
            var source = $"class Test {{{original}}}";
            var expectedCode = $"class Test {{{injected}}}";

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<LocalFunctionStatementSyntax>(source, expectedCode, MutantPlacer.ConvertExpressionToBody);
        }

        [Theory]
        [InlineData("() => Call(2)", "() => {return Call(2);}")]
        [InlineData("(x) => Call(2)", "(x) => {return Call(2);}")]
        [InlineData("x => Call(2)", "x => {return Call(2);}")]
        [InlineData("(out x) => Call(out x)", "(out x) => {return Call(out x);}")]
        [InlineData("(x, y) => Call(2)", "(x, y) => {return Call(2);}")]
        public void ShouldConvertAccessorExpressionBodyBackAndForth(string original, string injected)
        {
            var source = $"class Test {{ private void Any(){{ Register({original});}}}}";
            var expectedCode = $"class Test {{ private void Any(){{ Register({injected});}}}}";

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<AnonymousFunctionExpressionSyntax>(source, expectedCode, MutantPlacer.ConvertExpressionToBody);
        }

        [Theory]
        [InlineData("public int X { get => 1;}", "public int X { get {return 1;}}")]
        public void ShouldConvertAnonymousFunctionExpressionBodyBackAndForth(string original, string injected)
        {
            var source = $"class Test {{{original}}}";
            var expectedCode = $"class Test {{{injected}}}";

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<AccessorDeclarationSyntax>(source, expectedCode, MutantPlacer.ConvertExpressionToBody);
        }

        [Fact]
        public void ShouldConvertPropertyExpressionBodyBackAndForth()
        {
            var source = "class Test {public int X => 1;}";
            var expected = "class Test {public int X {get{return 1;}}}";

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<PropertyDeclarationSyntax>(source, expected, MutantPlacer.ConvertPropertyExpressionToBodyAccessor);
        }

        [Fact]
        public void ShouldInjectReturnAndRestore()
        {
            var source = "class Test {bool Method() {x++;}}";
            var expected = "class Test {bool Method() {x++;return default(bool);}}";

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<BaseMethodDeclarationSyntax, BlockSyntax>(source, expected, MutantPlacer.AddEndingReturn);
        }

        [Fact]
        public void ShouldInjectInitializersAndRestore()
        {
            var source = "class Test {bool Method(out int x) {x=0;}}";
            var expected = "class Test {bool Method(out int x) {{x = default(int);}x=0;}}";

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<BlockSyntax>(source, expected,
                (n) => MutantPlacer.AddDefaultInitializers(n,
                    new[]{SyntaxFactory.Parameter(SyntaxFactory.Identifier("x")).WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))
                    )}));
        }

        [Fact]
        public void ShouldInjectReturnToLocalFunctionAndRestore()
        {
            var source = "class Test {void Method(){ bool Method() {x++;};}}";
            var expected = "class Test {void Method(){ bool Method() {x++;return default(bool);};}}";

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<LocalFunctionStatementSyntax, BlockSyntax>(source, expected, MutantPlacer.AddEndingReturn);
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
            var actualNode = orchestrator.Mutate(CSharpSyntaxTree.ParseText(source).GetRoot());

            var node = actualNode.DescendantNodes().First(t => t is BlockSyntax);

            // Remove marker
            var restored = MutantPlacer.RemoveMutant(node);
            actualNode = actualNode.ReplaceNode(node, restored);

            // remove mutation
            node = actualNode.DescendantNodes().First(t => t.IsKind(SyntaxKind.IfStatement));
            restored = MutantPlacer.RemoveMutant(node);
            actualNode = actualNode.ReplaceNode(node, restored);

            // remove expression to body conversion
            node = actualNode.DescendantNodes().First(t => t is ConstructorDeclarationSyntax);
            restored = MutantPlacer.RemoveMutant(node);
            actualNode = actualNode.ReplaceNode(node, restored);

            var expectedNode = CSharpSyntaxTree.ParseText(source.Replace("StrykerNamespace", codeInjection.HelperNamespace)).GetRoot();
            expectedNode.ShouldNotContainErrors();
            actualNode.ShouldBeSemantically(expectedNode);
            actualNode.ShouldNotContainErrors();
        }
    }
}
