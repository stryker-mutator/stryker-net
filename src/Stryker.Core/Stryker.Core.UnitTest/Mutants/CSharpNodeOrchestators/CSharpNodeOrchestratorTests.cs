using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;
using Stryker.Core.Mutants.CsharpNodeOrchestrators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants.CSharpNodeOrchestators;
public class CSharpNodeOrchestratorTests : TestBase
{
    public CSharpNodeOrchestratorTests()
    {
        codeInjection = new CodeInjection();
        placer = new MutantPlacer(new CodeInjection());
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

        var orchestator = new BaseMethodDeclarationOrchestrator<BaseMethodDeclarationSyntax>(placer);

        CheckMutantPlacerProperlyPlaceAndRemoveHelpers<BaseMethodDeclarationSyntax>(source, expectedCode, orchestator.ConvertToBlockBody);
    }

    [Theory]
    [InlineData("void TestClass(){ void LocalFunction() => Value-='a';}", "void TestClass(){ void LocalFunction() {Value-='a';};}}")]
    [InlineData("void TestClass(){ int LocalFunction() => 4;}", "void TestClass(){ int LocalFunction() {return 4;};}")]
    public void ShouldConvertExpressionBodyBackLocalFunctionAndForth(string original, string injected)
    {
        var source = $"class Test {{{original}}}";
        var expectedCode = $"class Test {{{injected}}}";
        var orchestator = new LocalFunctionStatementOrchestrator(placer);

        CheckMutantPlacerProperlyPlaceAndRemoveHelpers<LocalFunctionStatementSyntax>(source, expectedCode, orchestator.ConvertToBlockBody);
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
        var orchestator = new AnonymousFunctionExpressionOrchestrator(placer);

        CheckMutantPlacerProperlyPlaceAndRemoveHelpers<AnonymousFunctionExpressionSyntax>(source, expectedCode, orchestator.ConvertToBlockBody);
    }

    [Theory]
    [InlineData("public int X { get => 1;}", "public int X { get {return 1;}}")]
    public void ShouldConvertAnonymousFunctionExpressionBodyBackAndForth(string original, string injected)
    {
        var source = $"class Test {{{original}}}";
        var expectedCode = $"class Test {{{injected}}}";
        var orchestator = new AccessorSyntaxOrchestrator(placer);

        CheckMutantPlacerProperlyPlaceAndRemoveHelpers<AccessorDeclarationSyntax>(source, expectedCode, orchestator.ConvertToBlockBody);
    }

    [Fact]
    public void ShouldConvertPropertyExpressionBodyBackAndForth()
    {
        var source = "class Test {public int X => 1;}";
        var expected = "class Test {public int X {get{return 1;}}}";
        var orchestator = new ExpressionBodiedPropertyOrchestrator(placer);

        CheckMutantPlacerProperlyPlaceAndRemoveHelpers<PropertyDeclarationSyntax>(source, expected, orchestator.ConvertToBlockBody);
    }
}
