using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class MutantPlacerTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        public void MutantPlacer_ShouldPlaceWithIfStatement(int id)
        {
            // 1 + 8;
            var originalNode = SyntaxFactory.ExpressionStatement(SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8))));

            // 1 - 8;
            var mutatedNode = SyntaxFactory.ExpressionStatement(SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8))));

            var result = MutantPlacer.PlaceStatementControlledMutations(originalNode, new[] {(id, (StatementSyntax) mutatedNode)});

            result.ToFullString().Replace(CodeInjection.HelperNamespace, "StrykerNamespace").ShouldBeSemantically("if (StrykerNamespace.MutantControl.IsActive("+id+@"))
            {
                1 - 8;
            } else {
                1 + 8;
            }");

            var removedResult = MutantPlacer.RemoveMutant(result);

            removedResult.ToString().ShouldBeSemantically(originalNode.ToString());
        }


        private void CheckMutantPlacerProperlyPlaceAndRemoveHelpers<T>(string sourceCode, string expectedCode,
            Func<T, T> placer) where T : SyntaxNode
        {
            var actualNode = CSharpSyntaxTree.ParseText(sourceCode).GetRoot();

            var node = actualNode.DescendantNodes().First(t => t is T) as T;
            // inject helper
            actualNode = actualNode.ReplaceNode(node, placer(node));
            actualNode.ToFullString().ShouldBeSemantically(expectedCode);

            node =
                actualNode.DescendantNodes().First(t => t is T) as T;
            // Remove helper
            var restored= MutantPlacer.RemoveMutant(node);
            actualNode = actualNode.ReplaceNode(node, restored);
            actualNode.ToFullString().ShouldBeSemantically(sourceCode);
            // try to remove again
            Should.Throw<InvalidOperationException>(() => MutantPlacer.RemoveMutant(restored));
        }

        [Theory]
        [InlineData(10)]
        [InlineData(16)]
        public void MutantPlacer_ShouldPlaceWithConditionalExpression(int id)
        {
            // 1 + 8;
            var originalNode = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            // 1 - 8;
            var mutatedNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var result = MutantPlacer.PlaceExpressionControlledMutations(originalNode, new[]{(id, (ExpressionSyntax) mutatedNode)});

            result.ToFullString()
                .ShouldBeSemantically(@$"({CodeInjection.HelperNamespace}.MutantControl.IsActive({id})?1-8:1+8)");

            var removedResult = MutantPlacer.RemoveMutant(result);

            removedResult.ToString().ShouldBeSemantically(originalNode.ToString());
        }

        [Theory]
        [InlineData("static TestClass()=> Value-='a';","static TestClass(){ Value-='a';}")]
        [InlineData("void TestClass()=> Value-='a';","void TestClass(){ Value-='a';}")]
        [InlineData("int TestClass()=> 1;","int TestClass(){ return 1;}")]
        [InlineData("~TestClass()=> Value-='a';","~TestClass(){ Value-='a';}")]
        [InlineData("public static operator int(Test t)=> 0;","public static operator int(Test t){ return 0;}")]
        [InlineData("public static int operator +(Test t, Test q)=> 0;","public static int operator +(Test t, Test q){return 0;}")]
        public void ShouldConvertExpressionBodyBackAndForth(string original, string injected)
        {
            var source = $"class Test {{{original}}}";
            var expectedCode = $"class Test {{{injected}}}";

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<BaseMethodDeclarationSyntax>(source, expectedCode, MutantPlacer.ConvertExpressionToBody);
        }

        [Theory]
        [InlineData("public int X { get => 1;}", "public int X { get {return 1;}}")]
        [InlineData("public int X { get => 1; set { }}", "public int X { get {return 1;} set { }}")]
        [InlineData("public int X { set => value++;}", "public int X { set { value++;}}")]
        public void ShouldConvertAccessorExpressionBodyBackAndForth(string original, string injected)
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

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<BaseMethodDeclarationSyntax>(source, expected, MutantPlacer.AddEndingReturn);
        }

        [Fact]
        public void ShouldInjectInitalizersAndRestore()
        {
            var source = "class Test {bool Method(out int x) {x=0;}}";
            var expected = "class Test {bool Method(out int x) {{x = default(int);}x=0;}}";

            CheckMutantPlacerProperlyPlaceAndRemoveHelpers<BaseMethodDeclarationSyntax>(source, expected,
                (n)=> MutantPlacer.AddDefaultInitialization(n, SyntaxFactory.Identifier("x"), SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))));
        }

        [Fact]
        public void ShouldRollBackFailedConstructor()
        {
            var source = @"class Test {
static TestClass()=> Value-='a';}";

            var orchestrator = new CsharpMutantOrchestrator(options: new StrykerOptions());
            var actualNode = orchestrator.Mutate(CSharpSyntaxTree.ParseText(source).GetRoot());

            var node = actualNode.DescendantNodes().First(t => t is BlockSyntax);

            // Remove marker
            var restored= MutantPlacer.RemoveMutant(node);
            actualNode = actualNode.ReplaceNode(node, restored);
            
            // remove mutation
            node = actualNode.DescendantNodes().First(t => t.Kind() == SyntaxKind.IfStatement);
            restored = MutantPlacer.RemoveMutant(node);
            actualNode = actualNode.ReplaceNode(node, restored);

            // remove expression to body conversion
            node = actualNode.DescendantNodes().First(t => t is ConstructorDeclarationSyntax);
            restored= MutantPlacer.RemoveMutant(node);
            actualNode = actualNode.ReplaceNode(node, restored);

            var expectedNode = CSharpSyntaxTree.ParseText(source.Replace("StrykerNamespace", CodeInjection.HelperNamespace)).GetRoot();
            expectedNode.ShouldNotContainErrors();
            actualNode.ShouldBeSemantically(expectedNode);
            actualNode.ShouldNotContainErrors();
        }
    }
}
